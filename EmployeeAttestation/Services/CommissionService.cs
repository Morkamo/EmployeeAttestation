using EmployeeAttestation.Data;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class CommissionService
{
    private readonly DatabaseManager databaseManager;

    public CommissionService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<Commission> GetAll(bool? isArchived = false) => QueryCommissions(null, isArchived);

    public List<Commission> Search(string query, bool? isArchived = false) => string.IsNullOrWhiteSpace(query)
        ? GetAll(isArchived)
        : QueryCommissions(query.Trim(), isArchived);

    public Commission? GetById(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, name, description, is_archived
                FROM commissions
                WHERE id = $id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadCommission(reader) : null;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionServiceException("Не удалось загрузить комиссию.", exception);
        }
    }

    public int Create(Commission commission)
    {
        ArgumentNullException.ThrowIfNull(commission);
        NormalizeAndValidate(commission);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO commissions (name, description, is_archived)
                VALUES ($name, $description, $isArchived);
                SELECT last_insert_rowid();
                """;
            AddParameters(command, commission);
            long id = (long)(command.ExecuteScalar()
                ?? throw new CommissionServiceException("Не удалось получить идентификатор комиссии."));
            commission.Id = checked((int)id);
            return commission.Id;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionServiceException("Не удалось сохранить комиссию.", exception);
        }
    }

    public void Update(Commission commission)
    {
        ArgumentNullException.ThrowIfNull(commission);
        ValidateId(commission.Id);
        NormalizeAndValidate(commission);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE commissions
                SET name = $name,
                    description = $description,
                    is_archived = $isArchived
                WHERE id = $id;
                """;
            AddParameters(command, commission);
            command.Parameters.AddWithValue("$id", commission.Id);
            if (command.ExecuteNonQuery() == 0) throw new CommissionServiceException("Комиссия не найдена.");
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionServiceException("Не удалось сохранить комиссию.", exception);
        }
    }

    public void Archive(int id) => SetArchived(id, true);

    public void Restore(int id) => SetArchived(id, false);

    private List<Commission> QueryCommissions(string? query, bool? isArchived)
    {
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, name, description, is_archived
                FROM commissions
                WHERE ($isArchived IS NULL OR is_archived = $isArchived)
                  AND ($query IS NULL OR name LIKE $query OR description LIKE $query)
                ORDER BY name;
                """;
            command.Parameters.AddWithValue("$isArchived", isArchived.HasValue ? (object)(isArchived.Value ? 1 : 0) : DBNull.Value);
            command.Parameters.AddWithValue("$query", query is null ? DBNull.Value : $"%{query}%");
            using SqliteDataReader reader = command.ExecuteReader();
            List<Commission> commissions = [];
            while (reader.Read()) commissions.Add(ReadCommission(reader));
            return commissions;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionServiceException("Не удалось загрузить комиссии.", exception);
        }
    }

    private void SetArchived(int id, bool isArchived)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "UPDATE commissions SET is_archived = $isArchived WHERE id = $id;";
            command.Parameters.AddWithValue("$isArchived", isArchived ? 1 : 0);
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0) throw new CommissionServiceException("Комиссия не найдена.");
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionServiceException(
                isArchived ? "Не удалось архивировать комиссию." : "Не удалось восстановить комиссию.",
                exception);
        }
    }

    private static Commission ReadCommission(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1),
        Description = reader.IsDBNull(2) ? null : reader.GetString(2),
        IsArchived = reader.GetInt32(3) == 1
    };

    private static void AddParameters(SqliteCommand command, Commission commission)
    {
        command.Parameters.AddWithValue("$name", commission.Name);
        command.Parameters.AddWithValue("$description", (object?)commission.Description ?? DBNull.Value);
        command.Parameters.AddWithValue("$isArchived", commission.IsArchived ? 1 : 0);
    }

    private static void NormalizeAndValidate(Commission commission)
    {
        commission.Name = commission.Name?.Trim() ?? string.Empty;
        commission.Description = string.IsNullOrWhiteSpace(commission.Description) ? null : commission.Description.Trim();
        if (commission.Name.Length == 0) throw new ArgumentException("Наименование комиссии обязательно.", nameof(commission));
    }

    private static void ValidateId(int id)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор комиссии должен быть положительным.");
    }

    private static bool IsDatabaseException(Exception exception) => exception is SqliteException or IOException or UnauthorizedAccessException;
}

public sealed class CommissionServiceException : Exception
{
    public CommissionServiceException(string message) : base(message) { }
    public CommissionServiceException(string message, Exception innerException) : base(message, innerException) { }
}
