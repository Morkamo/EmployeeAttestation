using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class EvaluationCriterionService
{
    private readonly DatabaseManager databaseManager;

    public EvaluationCriterionService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<EvaluationCriterion> GetAll(bool? isActive = null, string? category = null) =>
        QueryCriteria(null, isActive, category);

    public List<EvaluationCriterion> GetActive(bool includeManagerial = true) => QueryCriteria(
        null,
        true,
        null,
        excludeManagersOnly: !includeManagerial);

    public List<EvaluationCriterion> Search(
        string query,
        bool? isActive = null,
        string? category = null) => string.IsNullOrWhiteSpace(query)
            ? GetAll(isActive, category)
            : QueryCriteria(query.Trim(), isActive, category);

    public EvaluationCriterion? GetById(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, code, category, name, minimum_score, maximum_score,
                       managers_only, sort_order, is_active
                FROM evaluation_criteria
                WHERE id = $id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadCriterion(reader) : null;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EvaluationCriterionServiceException("Не удалось загрузить критерий.", exception);
        }
    }

    public int Create(EvaluationCriterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);
        NormalizeAndValidate(criterion);
        criterion.Code = $"CUSTOM_{Guid.NewGuid():N}".ToUpperInvariant();
        criterion.IsActive = true;
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO evaluation_criteria
                    (code, category, name, minimum_score, maximum_score,
                     managers_only, sort_order, is_active)
                VALUES
                    ($code, $category, $name, $minimumScore, $maximumScore,
                     $managersOnly, $sortOrder, 1);
                SELECT last_insert_rowid();
                """;
            AddParameters(command, criterion);
            criterion.Id = checked((int)(long)(command.ExecuteScalar()
                ?? throw new EvaluationCriterionServiceException("Не удалось получить идентификатор критерия.")));
            return criterion.Id;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EvaluationCriterionServiceException("Не удалось сохранить критерий.", exception);
        }
    }

    public void Update(EvaluationCriterion criterion)
    {
        ArgumentNullException.ThrowIfNull(criterion);
        ValidateId(criterion.Id);
        NormalizeAndValidate(criterion);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE evaluation_criteria
                SET category = $category,
                    name = $name,
                    minimum_score = $minimumScore,
                    maximum_score = $maximumScore,
                    managers_only = $managersOnly,
                    sort_order = $sortOrder
                WHERE id = $id;
                """;
            AddParameters(command, criterion, includeCode: false);
            command.Parameters.AddWithValue("$id", criterion.Id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new EvaluationCriterionServiceException("Критерий не найден.");
            }
        }
        catch (EvaluationCriterionServiceException)
        {
            throw;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EvaluationCriterionServiceException("Не удалось сохранить критерий.", exception);
        }
    }

    public bool DeleteOrDeactivate(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            using SqliteCommand usageCommand = connection.CreateCommand();
            usageCommand.Transaction = transaction;
            usageCommand.CommandText =
                "SELECT EXISTS(SELECT 1 FROM attestation_criteria WHERE criterion_id = $id);";
            usageCommand.Parameters.AddWithValue("$id", id);
            bool isUsed = Convert.ToInt32(usageCommand.ExecuteScalar()) == 1;

            using SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = isUsed
                ? "UPDATE evaluation_criteria SET is_active = 0 WHERE id = $id;"
                : "DELETE FROM evaluation_criteria WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new EvaluationCriterionServiceException("Критерий не найден.");
            }
            transaction.Commit();
            return !isUsed;
        }
        catch (EvaluationCriterionServiceException)
        {
            throw;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EvaluationCriterionServiceException("Не удалось удалить критерий.", exception);
        }
    }

    public void Activate(int id) => SetActive(id, true);

    public void Deactivate(int id) => SetActive(id, false);

    private List<EvaluationCriterion> QueryCriteria(
        string? query,
        bool? isActive,
        string? category,
        bool excludeManagersOnly = false)
    {
        if (category is not null && !EvaluationCategoryHelper.IsValid(category))
        {
            throw new ArgumentException("Неизвестная категория критерия.", nameof(category));
        }
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, code, category, name, minimum_score, maximum_score,
                       managers_only, sort_order, is_active
                FROM evaluation_criteria
                WHERE ($isActive IS NULL OR is_active = $isActive)
                  AND ($category IS NULL OR category = $category)
                  AND ($excludeManagersOnly = 0 OR managers_only = 0)
                  AND ($query IS NULL OR name LIKE $query OR code LIKE $query)
                ORDER BY CASE category
                             WHEN 'Professional' THEN 1
                             WHEN 'Personal' THEN 2
                             WHEN 'Managerial' THEN 3
                             ELSE 4
                         END,
                         sort_order,
                         name;
                """;
            command.Parameters.AddWithValue(
                "$isActive",
                isActive.HasValue ? (isActive.Value ? 1 : 0) : DBNull.Value);
            command.Parameters.AddWithValue("$category", category is null ? DBNull.Value : category);
            command.Parameters.AddWithValue("$excludeManagersOnly", excludeManagersOnly ? 1 : 0);
            command.Parameters.AddWithValue("$query", query is null ? DBNull.Value : $"%{query}%");
            using SqliteDataReader reader = command.ExecuteReader();
            List<EvaluationCriterion> criteria = [];
            while (reader.Read()) criteria.Add(ReadCriterion(reader));
            return criteria;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EvaluationCriterionServiceException("Не удалось загрузить критерии.", exception);
        }
    }

    private void SetActive(int id, bool isActive)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE evaluation_criteria SET is_active = $isActive WHERE id = $id;";
            command.Parameters.AddWithValue("$isActive", isActive ? 1 : 0);
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new EvaluationCriterionServiceException("Критерий не найден.");
            }
        }
        catch (EvaluationCriterionServiceException)
        {
            throw;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EvaluationCriterionServiceException(
                isActive ? "Не удалось восстановить критерий." : "Не удалось отключить критерий.",
                exception);
        }
    }

    private static EvaluationCriterion ReadCriterion(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Code = reader.GetString(1),
        Category = reader.GetString(2),
        Name = reader.GetString(3),
        MinimumScore = reader.GetInt32(4),
        MaximumScore = reader.GetInt32(5),
        ManagersOnly = reader.GetInt32(6) == 1,
        SortOrder = reader.GetInt32(7),
        IsActive = reader.GetInt32(8) == 1
    };

    private static void AddParameters(
        SqliteCommand command,
        EvaluationCriterion criterion,
        bool includeCode = true)
    {
        if (includeCode) command.Parameters.AddWithValue("$code", criterion.Code);
        command.Parameters.AddWithValue("$category", criterion.Category);
        command.Parameters.AddWithValue("$name", criterion.Name);
        command.Parameters.AddWithValue("$minimumScore", criterion.MinimumScore);
        command.Parameters.AddWithValue("$maximumScore", criterion.MaximumScore);
        command.Parameters.AddWithValue("$managersOnly", criterion.ManagersOnly ? 1 : 0);
        command.Parameters.AddWithValue("$sortOrder", criterion.SortOrder);
    }

    private static void NormalizeAndValidate(EvaluationCriterion criterion)
    {
        criterion.Name = criterion.Name?.Trim() ?? string.Empty;
        if (criterion.Name.Length == 0)
        {
            throw new ArgumentException("Введите наименование критерия.", nameof(criterion));
        }
        if (!EvaluationCategoryHelper.IsValid(criterion.Category))
        {
            throw new ArgumentException("Выберите категорию критерия.", nameof(criterion));
        }
        if (criterion.MinimumScore < 1 || criterion.MaximumScore > 5
            || criterion.MinimumScore > criterion.MaximumScore)
        {
            throw new ArgumentException("Диапазон баллов должен находиться в пределах 1–5.", nameof(criterion));
        }
        if (criterion.SortOrder < 0)
        {
            throw new ArgumentException("Порядок не может быть отрицательным.", nameof(criterion));
        }
    }

    private static void ValidateId(int id)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id));
    }

    private static bool IsDatabaseException(Exception exception) =>
        exception is SqliteException or IOException or UnauthorizedAccessException;
}

public sealed class EvaluationCriterionServiceException : Exception
{
    public EvaluationCriterionServiceException(string message) : base(message) { }
    public EvaluationCriterionServiceException(string message, Exception innerException) : base(message, innerException) { }
}
