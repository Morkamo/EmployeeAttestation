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

    public List<AttestationListItem> GetAll(string? status = null) => QueryAttestations(null, status);

    public List<AttestationListItem> Search(string query, string? status = null) => string.IsNullOrWhiteSpace(query)
        ? GetAll(status)
        : QueryAttestations(query.Trim(), status);

    public Attestation? GetById(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, employee_id, commission_id, attestation_date, status,
                       evaluate_managerial, created_at
                FROM attestations
                WHERE id = $id
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

    public int SaveDraft(Attestation attestation)
    {
        ArgumentNullException.ThrowIfNull(attestation);
        return attestation.Id == 0
            ? Create(attestation, AttestationStatusHelper.Draft, requireComposition: false)
            : Update(attestation, AttestationStatusHelper.Draft, AttestationStatusHelper.Draft, requireComposition: false);
    }

    public int SaveScheduled(Attestation attestation)
    {
        ArgumentNullException.ThrowIfNull(attestation);
        return attestation.Id == 0
            ? Create(attestation, AttestationStatusHelper.Scheduled, requireComposition: true)
            : Update(attestation, AttestationStatusHelper.Scheduled, AttestationStatusHelper.Draft, requireComposition: true);
    }

    public int UpdateScheduled(Attestation attestation)
    {
        ArgumentNullException.ThrowIfNull(attestation);
        ValidateId(attestation.Id);
        return Update(attestation, AttestationStatusHelper.Scheduled, AttestationStatusHelper.Scheduled, requireComposition: true);
    }

    public void Start(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            int compositionCount = GetCompositionCount(connection, transaction, GetCommissionId(connection, transaction, id));
            if (compositionCount == 0)
            {
                throw new AttestationServiceException(
                    "Невозможно начать аттестацию: в комиссии нет участников.");
            }

            using SqliteCommand command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText =
                """
                UPDATE attestations
                SET status = $newStatus,
                    commission_members_count = $compositionCount
                WHERE id = $id AND status = $currentStatus;
                """;
            command.Parameters.AddWithValue("$newStatus", AttestationStatusHelper.InProgress);
            command.Parameters.AddWithValue("$compositionCount", compositionCount);
            command.Parameters.AddWithValue("$id", id);
            command.Parameters.AddWithValue("$currentStatus", AttestationStatusHelper.Scheduled);
            if (command.ExecuteNonQuery() == 0)
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
        ValidateId(id);
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

    private List<AttestationListItem> QueryAttestations(string? query, string? status)
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
                SELECT a.id,
                       e.last_name,
                       e.first_name,
                       e.middle_name,
                       d.name,
                       p.name,
                       c.name,
                       a.attestation_date,
                       a.status,
                       a.evaluate_managerial
                FROM attestations AS a
                INNER JOIN employees AS e ON e.id = a.employee_id
                INNER JOIN departments AS d ON d.id = e.department_id
                INNER JOIN positions AS p ON p.id = e.position_id
                INNER JOIN commissions AS c ON c.id = a.commission_id
                WHERE ($status IS NULL OR a.status = $status)
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
            command.Parameters.AddWithValue("$query", query is null ? DBNull.Value : $"%{query}%");
            using SqliteDataReader reader = command.ExecuteReader();
            List<AttestationListItem> attestations = [];
            while (reader.Read())
            {
                attestations.Add(new AttestationListItem
                {
                    Id = reader.GetInt32(0),
                    EmployeeFullName = BuildFullName(
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.IsDBNull(3) ? null : reader.GetString(3)),
                    DepartmentName = reader.GetString(4),
                    PositionName = reader.GetString(5),
                    CommissionName = reader.GetString(6),
                    AttestationDate = ReadNullableDate(reader, 7),
                    Status = reader.GetString(8),
                    EvaluateManagerial = reader.GetInt32(9) == 1
                });
            }
            return attestations;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось загрузить аттестации.", exception);
        }
    }

    private int Create(Attestation attestation, string status, bool requireComposition)
    {
        NormalizeAndValidate(attestation, requireDate: status == AttestationStatusHelper.Scheduled);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            ValidateReferences(connection, transaction, attestation.EmployeeId, attestation.CommissionId, requireActive: true);
            if (requireComposition) EnsureCompositionExists(connection, transaction, attestation.CommissionId);

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
            long id = (long)(command.ExecuteScalar()
                ?? throw new AttestationServiceException("Не удалось получить идентификатор аттестации."));
            transaction.Commit();
            attestation.Id = checked((int)id);
            return attestation.Id;
        }
        catch (AttestationServiceException)
        {
            throw;
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new AttestationServiceException("Выбранный сотрудник или комиссия больше не существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось сохранить аттестацию.", exception);
        }
    }

    private int Update(
        Attestation attestation,
        string targetStatus,
        string allowedCurrentStatus,
        bool requireComposition)
    {
        ValidateId(attestation.Id);
        NormalizeAndValidate(attestation, requireDate: targetStatus == AttestationStatusHelper.Scheduled);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteTransaction transaction = connection.BeginTransaction();
            ValidateReferences(connection, transaction, attestation.EmployeeId, attestation.CommissionId, requireActive: false);
            if (requireComposition) EnsureCompositionExists(connection, transaction, attestation.CommissionId);

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
            transaction.Commit();
            return attestation.Id;
        }
        catch (AttestationServiceException)
        {
            throw;
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new AttestationServiceException("Выбранный сотрудник или комиссия больше не существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new AttestationServiceException("Не удалось сохранить аттестацию.", exception);
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
        if (GetCompositionCount(connection, transaction, commissionId) == 0)
        {
            throw new AttestationServiceException(
                "Невозможно запланировать аттестацию: в комиссии нет участников.");
        }
    }

    private static int GetCompositionCount(
        SqliteConnection connection,
        SqliteTransaction transaction,
        int commissionId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT COUNT(*) FROM commission_composition WHERE commission_id = $commissionId;";
        command.Parameters.AddWithValue("$commissionId", commissionId);
        return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static int GetCommissionId(SqliteConnection connection, SqliteTransaction transaction, int attestationId)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            "SELECT commission_id FROM attestations WHERE id = $id AND status = $status LIMIT 1;";
        command.Parameters.AddWithValue("$id", attestationId);
        command.Parameters.AddWithValue("$status", AttestationStatusHelper.Scheduled);
        object? result = command.ExecuteScalar();
        if (result is null)
        {
            throw new AttestationServiceException(
                "Начать можно только запланированную аттестацию. Обновите список и повторите попытку.");
        }
        return Convert.ToInt32(result, CultureInfo.InvariantCulture);
    }

    private static Attestation ReadAttestation(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        EmployeeId = reader.GetInt32(1),
        CommissionId = reader.GetInt32(2),
        AttestationDate = ReadNullableDate(reader, 3),
        Status = reader.GetString(4),
        EvaluateManagerial = reader.GetInt32(5) == 1,
        CreatedAt = ParseDateTime(reader.GetString(6))
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
        if (attestation.EmployeeId <= 0)
        {
            throw new ArgumentException("Выберите сотрудника.", nameof(attestation));
        }
        if (attestation.CommissionId <= 0)
        {
            throw new ArgumentException("Выберите комиссию.", nameof(attestation));
        }
        if (requireDate && !attestation.AttestationDate.HasValue)
        {
            throw new ArgumentException("Выберите дату аттестации.", nameof(attestation));
        }
        if (attestation.AttestationDate.HasValue)
        {
            attestation.AttestationDate = attestation.AttestationDate.Value.Date;
        }
    }

    private static DateTime? ReadNullableDate(SqliteDataReader reader, int ordinal)
    {
        if (reader.IsDBNull(ordinal)) return null;
        string value = reader.GetString(ordinal);
        return DateTime.TryParseExact(
            value,
            DateFormat,
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out DateTime result)
                ? result
                : DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out result)
                    ? result
                    : null;
    }

    private static DateTime ParseDateTime(string value) => DateTime.TryParse(
        value,
        CultureInfo.InvariantCulture,
        DateTimeStyles.RoundtripKind,
        out DateTime result)
            ? result
            : DateTime.MinValue;

    private static string BuildFullName(string lastName, string firstName, string? middleName) => string.Join(
        " ",
        new[] { lastName, firstName, middleName }.Where(value => !string.IsNullOrWhiteSpace(value)));

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор аттестации должен быть положительным.");
        }
    }

    private static bool IsDatabaseException(Exception exception) =>
        exception is SqliteException or IOException or UnauthorizedAccessException;
}

public sealed class AttestationServiceException : Exception
{
    public AttestationServiceException(string message) : base(message) { }

    public AttestationServiceException(string message, Exception innerException) : base(message, innerException) { }
}
