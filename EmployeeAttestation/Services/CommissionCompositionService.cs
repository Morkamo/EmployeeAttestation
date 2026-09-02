using EmployeeAttestation.Data;
using EmployeeAttestation.Extra;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class CommissionCompositionService
{
    private const int SqliteConstraintUnique = 2067;
    private readonly DatabaseManager databaseManager;

    public CommissionCompositionService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<CommissionComposition> GetComposition(int commissionId)
    {
        ValidateId(commissionId, nameof(commissionId));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT cc.id,
                       cc.commission_id,
                       cc.commission_member_id,
                       cc.role,
                       cc.sort_order,
                       cm.last_name,
                       cm.first_name,
                       cm.middle_name
                FROM commission_composition AS cc
                INNER JOIN commission_members AS cm ON cm.id = cc.commission_member_id
                WHERE cc.commission_id = $commissionId
                ORDER BY cc.sort_order, cm.last_name, cm.first_name, cm.middle_name;
                """;
            command.Parameters.AddWithValue("$commissionId", commissionId);
            using SqliteDataReader reader = command.ExecuteReader();
            List<CommissionComposition> composition = [];
            while (reader.Read())
            {
                composition.Add(new CommissionComposition
                {
                    Id = reader.GetInt32(0),
                    CommissionId = reader.GetInt32(1),
                    CommissionMemberId = reader.GetInt32(2),
                    Role = reader.GetString(3),
                    SortOrder = reader.GetInt32(4),
                    CommissionMemberName = BuildFullName(
                        reader.GetString(5),
                        reader.GetString(6),
                        reader.IsDBNull(7) ? null : reader.GetString(7))
                });
            }
            return composition;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionCompositionServiceException("Не удалось загрузить состав комиссии.", exception);
        }
    }

    public int AddMember(int commissionId, int commissionMemberId, string role, int sortOrder)
    {
        ValidateValues(commissionId, commissionMemberId, role, sortOrder);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO commission_composition
                    (commission_id, commission_member_id, role, sort_order)
                VALUES
                    ($commissionId, $commissionMemberId, $role, $sortOrder);
                SELECT last_insert_rowid();
                """;
            command.Parameters.AddWithValue("$commissionId", commissionId);
            command.Parameters.AddWithValue("$commissionMemberId", commissionMemberId);
            command.Parameters.AddWithValue("$role", role);
            command.Parameters.AddWithValue("$sortOrder", sortOrder);
            return checked((int)(long)(command.ExecuteScalar()
                ?? throw new CommissionCompositionServiceException("Не удалось получить идентификатор записи состава.")));
        }
        catch (SqliteException exception) when (exception.SqliteExtendedErrorCode == SqliteConstraintUnique)
        {
            throw new CommissionCompositionServiceException("Этот человек уже входит в состав комиссии.", exception);
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new CommissionCompositionServiceException("Выбранная комиссия или член комиссии больше не существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionCompositionServiceException("Не удалось добавить человека в состав комиссии.", exception);
        }
    }

    public void UpdateMember(int compositionId, string role, int sortOrder)
    {
        ValidateId(compositionId, nameof(compositionId));
        ValidateRoleAndOrder(role, sortOrder);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE commission_composition
                SET role = $role,
                    sort_order = $sortOrder
                WHERE id = $id;
                """;
            command.Parameters.AddWithValue("$role", role);
            command.Parameters.AddWithValue("$sortOrder", sortOrder);
            command.Parameters.AddWithValue("$id", compositionId);
            if (command.ExecuteNonQuery() == 0) throw new CommissionCompositionServiceException("Запись состава не найдена.");
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionCompositionServiceException("Не удалось изменить участника комиссии.", exception);
        }
    }

    public void RemoveMember(int compositionId)
    {
        ValidateId(compositionId, nameof(compositionId));
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM commission_composition WHERE id = $id;";
            command.Parameters.AddWithValue("$id", compositionId);
            if (command.ExecuteNonQuery() == 0) throw new CommissionCompositionServiceException("Запись состава не найдена.");
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionCompositionServiceException("Не удалось удалить участника из состава комиссии.", exception);
        }
    }

    private static void ValidateValues(int commissionId, int memberId, string role, int sortOrder)
    {
        ValidateId(commissionId, nameof(commissionId));
        ValidateId(memberId, nameof(memberId));
        ValidateRoleAndOrder(role, sortOrder);
    }

    private static void ValidateRoleAndOrder(string role, int sortOrder)
    {
        if (!CommissionRoleHelper.IsValidRole(role)) throw new ArgumentException("Выберите роль участника.", nameof(role));
        if (sortOrder < 0) throw new ArgumentOutOfRangeException(nameof(sortOrder), "Порядок не может быть отрицательным.");
    }

    private static void ValidateId(int id, string parameterName)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(parameterName, "Идентификатор должен быть положительным.");
    }

    private static string BuildFullName(string lastName, string firstName, string? middleName) => string.Join(
        " ",
        new[] { lastName, firstName, middleName }.Where(value => !string.IsNullOrWhiteSpace(value)));

    private static bool IsDatabaseException(Exception exception) => exception is SqliteException or IOException or UnauthorizedAccessException;
}

public sealed class CommissionCompositionServiceException : Exception
{
    public CommissionCompositionServiceException(string message) : base(message) { }
    public CommissionCompositionServiceException(string message, Exception innerException) : base(message, innerException) { }
}
