using System.Globalization;
using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class AttestationService
{
    private const string DateFormat = "yyyy-MM-dd";
    private const string DateTimeFormat = "yyyy-MM-dd'T'HH:mm:ss.fffK";
    private readonly DatabaseManager databaseManager;

    public AttestationService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<AttestationListItem> GetAll(string? status = null) => QueryAttestations(null, status, null);

    public List<AttestationListItem> Search(string query, string? status = null) => string.IsNullOrWhiteSpace(query)
        ? GetAll(status)
        : QueryAttestations(query.Trim(), status, null);

    public List<AttestationListItem> GetEmployeeHistory(int employeeId)
    {
        ValidateId(employeeId, nameof(employeeId));
        return QueryAttestations(null, null, employeeId);
    }

    public Attestation? GetById(int id)
    {
        ValidateId(id, nameof(id));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT a.id, a.employee_id, a.commission_id, a.attestation_date, a.status,
                       a.evaluate_managerial, a.created_at,
                       a.professional_average, a.personal_average, a.managerial_average,
                       a.overall_average, a.decision, a.recommendations,
                       a.commission_members_count, a.present_members_count,
                       a.votes_for, a.votes_against, a.votes_abstained, a.completed_at,
                       e.last_name, e.first_name, e.middle_name,
                       d.name, p.name, c.name
                FROM attestations AS a
                INNER JOIN employees AS e ON e.id = a.employee_id
                INNER JOIN departments AS d ON d.id = e.department_id
                INNER JOIN positions AS p ON p.id = e.position_id
                INNER JOIN commissions AS c ON c.id = a.commission_id
                WHERE a.id = $id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadAttestation(reader) : null;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось загрузить аттестацию.", exception);
        }
    }

    public List<AttestationCriterion> GetCriteria(int attestationId)
    {
        ValidateId(attestationId, nameof(attestationId));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, attestation_id, criterion_id, criterion_code, criterion_name,
                       category, minimum_score, maximum_score, managers_only, sort_order
                FROM attestation_criteria
                WHERE attestation_id = $attestationId
                ORDER BY CASE category
                             WHEN 'Professional' THEN 1
                             WHEN 'Personal' THEN 2
                             WHEN 'Managerial' THEN 3
                             ELSE 4
                         END,
                         sort_order,
                         criterion_name;
                """;
            command.Parameters.AddWithValue("$attestationId", attestationId);
            using SqliteDataReader reader = command.ExecuteReader();
            List<AttestationCriterion> result = [];
            while (reader.Read()) result.Add(ReadAttestationCriterion(reader));
            return result;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось загрузить критерии аттестации.", exception);
        }
    }

    public int SaveDraft(Attestation attestation) => SaveDraft(attestation, []);

    public int SaveDraft(Attestation attestation, IReadOnlyCollection<int> criterionIds)
    {
        ArgumentNullException.ThrowIfNull(attestation);
        ArgumentNullException.ThrowIfNull(criterionIds);
        return attestation.Id == 0
            ? Create(attestation, criterionIds, AttestationStatusHelper.Draft)
            : Update(attestation, criterionIds, AttestationStatusHelper.Draft, AttestationStatusHelper.Draft);
    }

    public int SaveScheduled(Attestation attestation) => SaveScheduled(attestation, []);

    public int SaveScheduled(Attestation attestation, IReadOnlyCollection<int> criterionIds)
    {
        ArgumentNullException.ThrowIfNull(attestation);
        ArgumentNullException.ThrowIfNull(criterionIds);
        return attestation.Id == 0
            ? Create(attestation, criterionIds, AttestationStatusHelper.Scheduled)
            : Update(attestation, criterionIds, AttestationStatusHelper.Scheduled, AttestationStatusHelper.Draft);
    }

    public int UpdateScheduled(Attestation attestation) => UpdateScheduled(attestation, []);

    public int UpdateScheduled(Attestation attestation, IReadOnlyCollection<int> criterionIds)
    {
        ArgumentNullException.ThrowIfNull(attestation);
        ArgumentNullException.ThrowIfNull(criterionIds);
        ValidateId(attestation.Id, nameof(attestation.Id));
        return Update(attestation, criterionIds, AttestationStatusHelper.Scheduled, AttestationStatusHelper.Scheduled);
    }

    public void Start(int id)
    {
        ValidateId(id, nameof(id));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            (int employeeId, int commissionId) = GetScheduledReferences(connection, transaction, id);
            ValidateReferences(connection, transaction, employeeId, commissionId, requireActive: false);

            int criterionCount = CountRows(
                connection,
                transaction,
                "attestation_criteria",
                "attestation_id",
                id);
            if (criterionCount == 0)
            {
                throw new AttestationServiceException("Выберите хотя бы один критерий аттестации.");
            }

            int compositionCount = CountRows(
                connection,
                transaction,
                "commission_composition",
                "commission_id",
                commissionId);
            if (compositionCount == 0)
            {
                throw new AttestationServiceException("Невозможно начать аттестацию: в комиссии нет участников.");
            }

            using (SqliteCommand clearCommand = connection.CreateCommand())
            {
                clearCommand.Transaction = transaction;
                clearCommand.CommandText =
                    "DELETE FROM attestation_commission_members WHERE attestation_id = $attestationId;";
                clearCommand.Parameters.AddWithValue("$attestationId", id);
                clearCommand.ExecuteNonQuery();
            }

            using (SqliteCommand snapshotCommand = connection.CreateCommand())
            {
                snapshotCommand.Transaction = transaction;
                snapshotCommand.CommandText =
                    """
                    INSERT INTO attestation_commission_members
                        (attestation_id, commission_member_id, member_full_name, role, sort_order, is_present)
                    SELECT $attestationId,
                           cm.id,
                           TRIM(cm.last_name || ' ' || cm.first_name || ' ' || IFNULL(cm.middle_name, '')),
                           cc.role,
                           cc.sort_order,
                           1
                    FROM commission_composition AS cc
                    INNER JOIN commission_members AS cm ON cm.id = cc.commission_member_id
                    WHERE cc.commission_id = $commissionId
                    ORDER BY cc.sort_order, cm.last_name, cm.first_name, cm.middle_name;
                    """;
                snapshotCommand.Parameters.AddWithValue("$attestationId", id);
                snapshotCommand.Parameters.AddWithValue("$commissionId", commissionId);
                if (snapshotCommand.ExecuteNonQuery() == 0)
                {
                    throw new AttestationServiceException("Невозможно начать аттестацию: в комиссии нет участников.");
                }
            }

            using SqliteCommand updateCommand = connection.CreateCommand();
            updateCommand.Transaction = transaction;
            updateCommand.CommandText =
                """
                UPDATE attestations
                SET status = $newStatus,
                    commission_members_count = $compositionCount
                WHERE id = $id AND status = $currentStatus;
                """;
            updateCommand.Parameters.AddWithValue("$newStatus", AttestationStatusHelper.InProgress);
            updateCommand.Parameters.AddWithValue("$compositionCount", compositionCount);
            updateCommand.Parameters.AddWithValue("$id", id);
            updateCommand.Parameters.AddWithValue("$currentStatus", AttestationStatusHelper.Scheduled);
            if (updateCommand.ExecuteNonQuery() == 0)
            {
                throw new AttestationServiceException(
                    "Начать можно только запланированную аттестацию. Обновите список и повторите попытку.");
            }
            transaction.Commit();
        }
        catch (AttestationServiceException)
        {
            throw;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось начать аттестацию.", exception);
        }
    }

    public void Cancel(int id)
    {
        ValidateId(id, nameof(id));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE attestations
                SET status = $cancelled
                WHERE id = $id AND status IN ($draft, $scheduled);
                """;
            command.Parameters.AddWithValue("$cancelled", AttestationStatusHelper.Cancelled);
            command.Parameters.AddWithValue("$draft", AttestationStatusHelper.Draft);
            command.Parameters.AddWithValue("$scheduled", AttestationStatusHelper.Scheduled);
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new AttestationServiceException(
                    "Отменить можно только черновик или запланированную аттестацию. Обновите список и повторите попытку.");
            }
        }
        catch (AttestationServiceException)
        {
            throw;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось отменить аттестацию.", exception);
        }
    }

    private List<AttestationListItem> QueryAttestations(string? query, string? status, int? employeeId)
    {
        if (!string.IsNullOrWhiteSpace(status) && !AttestationStatusHelper.IsValid(status))
        {
            throw new ArgumentException("Неизвестный статус аттестации.", nameof(status));
        }
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT a.id, e.last_name, e.first_name, e.middle_name,
                       d.name, p.name, c.name, a.attestation_date, a.status,
                       a.evaluate_managerial, a.overall_average, a.decision
                FROM attestations AS a
                INNER JOIN employees AS e ON e.id = a.employee_id
                INNER JOIN departments AS d ON d.id = e.department_id
                INNER JOIN positions AS p ON p.id = e.position_id
                INNER JOIN commissions AS c ON c.id = a.commission_id
                WHERE ($status IS NULL OR a.status = $status)
                  AND ($employeeId IS NULL OR a.employee_id = $employeeId)
                  AND ($query IS NULL
                       OR e.last_name LIKE $query
                       OR e.first_name LIKE $query
                       OR e.middle_name LIKE $query
                       OR (e.last_name || ' ' || e.first_name || ' ' || IFNULL(e.middle_name, '')) LIKE $query
                       OR d.name LIKE $query
                       OR p.name LIKE $query
                       OR c.name LIKE $query)
                ORDER BY a.attestation_date IS NULL,
                         a.attestation_date DESC,
                         a.id DESC;
                """;
            command.Parameters.AddWithValue("$status", string.IsNullOrWhiteSpace(status) ? DBNull.Value : status);
            command.Parameters.AddWithValue("$employeeId", employeeId.HasValue ? employeeId.Value : DBNull.Value);
            command.Parameters.AddWithValue("$query", query is null ? DBNull.Value : $"%{query}%");
            using SqliteDataReader reader = command.ExecuteReader();
            List<AttestationListItem> result = [];
            while (reader.Read())
            {
                result.Add(new AttestationListItem
                {
                    Id = reader.GetInt32(0),
                    EmployeeFullName = BuildFullName(
                        reader.GetString(1), reader.GetString(2), reader.IsDBNull(3) ? null : reader.GetString(3)),
                    DepartmentName = reader.GetString(4),
                    PositionName = reader.GetString(5),
                    CommissionName = reader.GetString(6),
                    AttestationDate = ReadNullableDate(reader, 7),
                    Status = reader.GetString(8),
                    EvaluateManagerial = reader.GetInt32(9) == 1,
                    OverallAverage = reader.IsDBNull(10) ? null : reader.GetDouble(10),
                    Decision = reader.IsDBNull(11) ? null : reader.GetString(11)
                });
            }
            return result;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось загрузить аттестации.", exception);
        }
    }

    private int Create(
        Attestation attestation,
        IReadOnlyCollection<int> criterionIds,
        string status)
    {
        NormalizeAndValidate(attestation, requireDate: status == AttestationStatusHelper.Scheduled);
        if (status == AttestationStatusHelper.Scheduled && criterionIds.Count == 0)
        {
            throw new AttestationServiceException("Выберите хотя бы один критерий аттестации.");
        }
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            ValidateReferences(connection, transaction, attestation.EmployeeId, attestation.CommissionId, requireActive: true);
            if (status == AttestationStatusHelper.Scheduled)
            {
                EnsureCompositionExists(connection, transaction, attestation.CommissionId);
            }

            attestation.Status = status;
            attestation.CreatedAt = DateTime.Now;
            using SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                """
                INSERT INTO attestations
                    (employee_id, commission_id, attestation_date, status, evaluate_managerial, created_at)
                VALUES
                    ($employeeId, $commissionId, $attestationDate, $status, $evaluateManagerial, $createdAt);
                SELECT last_insert_rowid();
                """;
            AddParameters(command, attestation);
            attestation.Id = checked((int)(long)(command.ExecuteScalar()
                ?? throw new AttestationServiceException("Не удалось получить идентификатор аттестации.")));
            ReplaceCriteriaSnapshot(connection, transaction, attestation.Id, criterionIds, attestation.EvaluateManagerial);
            transaction.Commit();
            return attestation.Id;
        }
        catch (AttestationServiceException)
        {
            throw;
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new AttestationServiceException("Выбранный сотрудник, комиссия или критерий больше не существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось сохранить аттестацию.", exception);
        }
    }

    private int Update(
        Attestation attestation,
        IReadOnlyCollection<int> criterionIds,
        string targetStatus,
        string allowedCurrentStatus)
    {
        ValidateId(attestation.Id, nameof(attestation.Id));
        NormalizeAndValidate(attestation, requireDate: targetStatus == AttestationStatusHelper.Scheduled);
        if (targetStatus == AttestationStatusHelper.Scheduled && criterionIds.Count == 0)
        {
            throw new AttestationServiceException("Выберите хотя бы один критерий аттестации.");
        }
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            ValidateReferences(connection, transaction, attestation.EmployeeId, attestation.CommissionId, requireActive: false);
            if (targetStatus == AttestationStatusHelper.Scheduled)
            {
                EnsureCompositionExists(connection, transaction, attestation.CommissionId);
            }

            using SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                """
                UPDATE attestations
                SET employee_id = $employeeId,
                    commission_id = $commissionId,
                    attestation_date = $attestationDate,
                    status = $status,
                    evaluate_managerial = $evaluateManagerial
                WHERE id = $id AND status = $allowedCurrentStatus;
                """;
            attestation.Status = targetStatus;
            AddParameters(command, attestation, includeCreatedAt: false);
            command.Parameters.AddWithValue("$id", attestation.Id);
            command.Parameters.AddWithValue("$allowedCurrentStatus", allowedCurrentStatus);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new AttestationServiceException(
                    "Статус аттестации изменился или запись больше не существует. Обновите список.");
            }
            ReplaceCriteriaSnapshot(connection, transaction, attestation.Id, criterionIds, attestation.EvaluateManagerial);
            transaction.Commit();
            return attestation.Id;
        }
        catch (AttestationServiceException)
        {
            throw;
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new AttestationServiceException("Выбранный сотрудник, комиссия или критерий больше не существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось сохранить аттестацию.", exception);
        }
    }

    private static void ReplaceCriteriaSnapshot(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int attestationId,
        IReadOnlyCollection<int> criterionIds,
        bool evaluateManagerial)
    {
        int[] ids = criterionIds.Where(id => id > 0).Distinct().ToArray();
        List<EvaluationCriterion> criteria = [];
        if (ids.Length > 0)
        {
            using SqliteCommand selectCommand = connection.CreateCommand();
            selectCommand.Transaction = transaction;
            string[] parameterNames = ids.Select((_, index) => $"$criterion{index}").ToArray();
            selectCommand.CommandText =
                $"""
                SELECT id, code, category, name, minimum_score, maximum_score,
                       managers_only, sort_order, is_active
                FROM evaluation_criteria
                WHERE id IN ({string.Join(", ", parameterNames)});
                """;
            for (int index = 0; index < ids.Length; index++)
            {
                selectCommand.Parameters.AddWithValue(parameterNames[index], ids[index]);
            }
            using SqliteDataReader reader = selectCommand.ExecuteReader();
            while (reader.Read())
            {
                criteria.Add(new EvaluationCriterion
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
                });
            }
            if (criteria.Count != ids.Length)
            {
                throw new AttestationServiceException("Один из выбранных критериев больше не существует.");
            }
            if (!evaluateManagerial && criteria.Any(item => item.ManagersOnly))
            {
                throw new AttestationServiceException(
                    "Руководительские критерии нельзя выбрать без признака «Оценивать как руководителя».");
            }
        }

        using (SqliteCommand deleteCommand = connection.CreateCommand())
        {
            deleteCommand.Transaction = transaction;
            deleteCommand.CommandText = "DELETE FROM attestation_criteria WHERE attestation_id = $attestationId;";
            deleteCommand.Parameters.AddWithValue("$attestationId", attestationId);
            deleteCommand.ExecuteNonQuery();
        }

        foreach (EvaluationCriterion criterion in criteria)
        {
            using SqliteCommand insertCommand = connection.CreateCommand();
            insertCommand.Transaction = transaction;
            insertCommand.CommandText =
                """
                INSERT INTO attestation_criteria
                    (attestation_id, criterion_id, criterion_code, criterion_name, category,
                     minimum_score, maximum_score, managers_only, sort_order)
                VALUES
                    ($attestationId, $criterionId, $code, $name, $category,
                     $minimumScore, $maximumScore, $managersOnly, $sortOrder);
                """;
            insertCommand.Parameters.AddWithValue("$attestationId", attestationId);
            insertCommand.Parameters.AddWithValue("$criterionId", criterion.Id);
            insertCommand.Parameters.AddWithValue("$code", criterion.Code);
            insertCommand.Parameters.AddWithValue("$name", criterion.Name);
            insertCommand.Parameters.AddWithValue("$category", criterion.Category);
            insertCommand.Parameters.AddWithValue("$minimumScore", criterion.MinimumScore);
            insertCommand.Parameters.AddWithValue("$maximumScore", criterion.MaximumScore);
            insertCommand.Parameters.AddWithValue("$managersOnly", criterion.ManagersOnly ? 1 : 0);
            insertCommand.Parameters.AddWithValue("$sortOrder", criterion.SortOrder);
            insertCommand.ExecuteNonQuery();
        }
    }

    private static void ValidateReferences(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int employeeId,
        int commissionId,
        bool requireActive)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT EXISTS(
                       SELECT 1 FROM employees
                       WHERE id = $employeeId AND ($requireActive = 0 OR is_archived = 0)
                   ),
                   EXISTS(
                       SELECT 1 FROM commissions
                       WHERE id = $commissionId AND ($requireActive = 0 OR is_archived = 0)
                   );
            """;
        command.Parameters.AddWithValue("$employeeId", employeeId);
        command.Parameters.AddWithValue("$commissionId", commissionId);
        command.Parameters.AddWithValue("$requireActive", requireActive ? 1 : 0);
        using SqliteDataReader reader = command.ExecuteReader();
        reader.Read();
        if (reader.GetInt32(0) == 0)
        {
            throw new AttestationServiceException(
                requireActive ? "Выберите действующего сотрудника." : "Выбранный сотрудник больше не существует.");
        }
        if (reader.GetInt32(1) == 0)
        {
            throw new AttestationServiceException(
                requireActive ? "Выберите действующую комиссию." : "Выбранная комиссия больше не существует.");
        }
    }

    private static void EnsureCompositionExists(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int commissionId)
    {
        if (CountRows(connection, transaction, "commission_composition", "commission_id", commissionId) == 0)
        {
            throw new AttestationServiceException(
                "Невозможно запланировать аттестацию: в комиссии нет участников.");
        }
    }

    private static int CountRows(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string tableName,
        string keyColumn,
        int value)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = $"SELECT COUNT(*) FROM {tableName} WHERE {keyColumn} = $value;";
        command.Parameters.AddWithValue("$value", value);
        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static (int EmployeeId, int CommissionId) GetScheduledReferences(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int attestationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            SELECT employee_id, commission_id
            FROM attestations
            WHERE id = $id AND status = $status
            LIMIT 1;
            """;
        command.Parameters.AddWithValue("$id", attestationId);
        command.Parameters.AddWithValue("$status", AttestationStatusHelper.Scheduled);
        using SqliteDataReader reader = command.ExecuteReader();
        if (!reader.Read())
        {
            throw new AttestationServiceException(
                "Начать можно только запланированную аттестацию. Обновите список и повторите попытку.");
        }
        return (reader.GetInt32(0), reader.GetInt32(1));
    }

    private static Attestation ReadAttestation(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        EmployeeId = reader.GetInt32(1),
        CommissionId = reader.GetInt32(2),
        AttestationDate = ReadNullableDate(reader, 3),
        Status = reader.GetString(4),
        EvaluateManagerial = reader.GetInt32(5) == 1,
        CreatedAt = ParseDateTime(reader.GetString(6)),
        ProfessionalAverage = reader.IsDBNull(7) ? null : reader.GetDouble(7),
        PersonalAverage = reader.IsDBNull(8) ? null : reader.GetDouble(8),
        ManagerialAverage = reader.IsDBNull(9) ? null : reader.GetDouble(9),
        OverallAverage = reader.IsDBNull(10) ? null : reader.GetDouble(10),
        Decision = reader.IsDBNull(11) ? null : reader.GetString(11),
        Recommendations = reader.IsDBNull(12) ? null : reader.GetString(12),
        CommissionMembersCount = reader.IsDBNull(13) ? null : reader.GetInt32(13),
        PresentMembersCount = reader.IsDBNull(14) ? null : reader.GetInt32(14),
        VotesFor = reader.IsDBNull(15) ? null : reader.GetInt32(15),
        VotesAgainst = reader.IsDBNull(16) ? null : reader.GetInt32(16),
        VotesAbstained = reader.IsDBNull(17) ? null : reader.GetInt32(17),
        CompletedAt = reader.IsDBNull(18) ? null : ParseDateTime(reader.GetString(18)),
        EmployeeFullName = BuildFullName(
            reader.GetString(19), reader.GetString(20), reader.IsDBNull(21) ? null : reader.GetString(21)),
        DepartmentName = reader.GetString(22),
        PositionName = reader.GetString(23),
        CommissionName = reader.GetString(24)
    };

    private static AttestationCriterion ReadAttestationCriterion(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        AttestationId = reader.GetInt32(1),
        CriterionId = reader.IsDBNull(2) ? null : reader.GetInt32(2),
        CriterionCode = reader.GetString(3),
        CriterionName = reader.GetString(4),
        Category = reader.GetString(5),
        MinimumScore = reader.GetInt32(6),
        MaximumScore = reader.GetInt32(7),
        ManagersOnly = reader.GetInt32(8) == 1,
        SortOrder = reader.GetInt32(9)
    };

    private static void AddParameters(SqliteCommand command, Attestation attestation, bool includeCreatedAt = true)
    {
        command.Parameters.AddWithValue("$employeeId", attestation.EmployeeId);
        command.Parameters.AddWithValue("$commissionId", attestation.CommissionId);
        command.Parameters.AddWithValue(
            "$attestationDate",
            attestation.AttestationDate.HasValue
                ? attestation.AttestationDate.Value.ToString(DateFormat, CultureInfo.InvariantCulture)
                : DBNull.Value);
        command.Parameters.AddWithValue("$status", attestation.Status);
        command.Parameters.AddWithValue("$evaluateManagerial", attestation.EvaluateManagerial ? 1 : 0);
        if (includeCreatedAt)
        {
            command.Parameters.AddWithValue(
                "$createdAt",
                attestation.CreatedAt.ToString(DateTimeFormat, CultureInfo.InvariantCulture));
        }
    }

    private static void NormalizeAndValidate(Attestation attestation, bool requireDate)
    {
        if (attestation.EmployeeId <= 0) throw new ArgumentException("Выберите сотрудника.", nameof(attestation));
        if (attestation.CommissionId <= 0) throw new ArgumentException("Выберите комиссию.", nameof(attestation));
        if (requireDate && !attestation.AttestationDate.HasValue)
        {
            throw new ArgumentException("Выберите дату аттестации.", nameof(attestation));
        }
        if (attestation.AttestationDate.HasValue) attestation.AttestationDate = attestation.AttestationDate.Value.Date;
    }

    private static DateTime? ReadNullableDate(SqliteDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        string value = reader.GetString(ordinal);
        return DateTime.TryParseExact(value, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result)
            ? result
            : DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result)
                ? result
                : null;
    }

    private static DateTime ParseDateTime(string value) => DateTime.TryParse(
        value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out DateTime result)
            ? result
            : DateTime.MinValue;

    private static string BuildFullName(string lastName, string firstName, string? middleName) => string.Join(
        " ", new[] { lastName, firstName, middleName }.Where(value => !string.IsNullOrWhiteSpace(value)));

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(parameterName, "Идентификатор должен быть положительным.");
    }

    private static bool IsDatabaseException(Exception exception) =>
        exception is SqliteException or IOException or UnauthorizedAccessException;
}

public sealed class AttestationServiceException : Exception
{
    public AttestationServiceException(string message) : base(message) { }
    public AttestationServiceException(string message, Exception innerException) : base(message, innerException) { }
}
