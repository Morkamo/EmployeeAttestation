using EmployeeAttestation.Data;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class CommissionMemberService
{
    private readonly DatabaseManager databaseManager;

    public CommissionMemberService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<CommissionMember> GetAll(bool? isArchived = false) => QueryMembers(null, isArchived);

    public List<CommissionMember> Search(string query, bool? isArchived = false) => string.IsNullOrWhiteSpace(query)
        ? GetAll(isArchived)
        : QueryMembers(query.Trim(), isArchived);

    public CommissionMember? GetById(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, last_name, first_name, middle_name, is_archived
                FROM commission_members
                WHERE id = $id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadMember(reader) : null;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionMemberServiceException("Не удалось загрузить члена комиссии.", exception);
        }
    }

    public int Create(CommissionMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        NormalizeAndValidate(member);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO commission_members (last_name, first_name, middle_name, is_archived)
                VALUES ($lastName, $firstName, $middleName, $isArchived);
                SELECT last_insert_rowid();
                """;
            AddParameters(command, member);
            long id = (long)(command.ExecuteScalar()
                ?? throw new CommissionMemberServiceException("Не удалось получить идентификатор члена комиссии."));
            member.Id = checked((int)id);
            return member.Id;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionMemberServiceException("Не удалось сохранить члена комиссии.", exception);
        }
    }

    public void Update(CommissionMember member)
    {
        ArgumentNullException.ThrowIfNull(member);
        ValidateId(member.Id);
        NormalizeAndValidate(member);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE commission_members
                SET last_name = $lastName,
                    first_name = $firstName,
                    middle_name = $middleName,
                    is_archived = $isArchived
                WHERE id = $id;
                """;
            AddParameters(command, member);
            command.Parameters.AddWithValue("$id", member.Id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new CommissionMemberServiceException("Член комиссии не найден.");
            }
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionMemberServiceException("Не удалось сохранить члена комиссии.", exception);
        }
    }

    public void Archive(int id) => SetArchived(id, true);

    public void Restore(int id) => SetArchived(id, false);

    private List<CommissionMember> QueryMembers(string? query, bool? isArchived)
    {
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, last_name, first_name, middle_name, is_archived
                FROM commission_members
                WHERE ($isArchived IS NULL OR is_archived = $isArchived)
                  AND ($query IS NULL
                       OR last_name LIKE $query
                       OR first_name LIKE $query
                       OR middle_name LIKE $query)
                ORDER BY last_name, first_name, middle_name;
                """;
            command.Parameters.AddWithValue("$isArchived", isArchived.HasValue ? (object)(isArchived.Value ? 1 : 0) : DBNull.Value);
            command.Parameters.AddWithValue("$query", query is null ? DBNull.Value : $"%{query}%");
            using SqliteDataReader reader = command.ExecuteReader();
            List<CommissionMember> members = [];
            while (reader.Read()) members.Add(ReadMember(reader));
            return members;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionMemberServiceException("Не удалось загрузить членов комиссии.", exception);
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
            command.CommandText = "UPDATE commission_members SET is_archived = $isArchived WHERE id = $id;";
            command.Parameters.AddWithValue("$isArchived", isArchived ? 1 : 0);
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new CommissionMemberServiceException("Член комиссии не найден.");
            }
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new CommissionMemberServiceException(
                isArchived ? "Не удалось архивировать члена комиссии." : "Не удалось восстановить члена комиссии.",
                exception);
        }
    }

    private static CommissionMember ReadMember(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        LastName = reader.GetString(1),
        FirstName = reader.GetString(2),
        MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
        IsArchived = reader.GetInt32(4) == 1
    };

    private static void AddParameters(SqliteCommand command, CommissionMember member)
    {
        command.Parameters.AddWithValue("$lastName", member.LastName);
        command.Parameters.AddWithValue("$firstName", member.FirstName);
        command.Parameters.AddWithValue("$middleName", (object?)member.MiddleName ?? DBNull.Value);
        command.Parameters.AddWithValue("$isArchived", member.IsArchived ? 1 : 0);
    }

    private static void NormalizeAndValidate(CommissionMember member)
    {
        member.LastName = member.LastName?.Trim() ?? string.Empty;
        member.FirstName = member.FirstName?.Trim() ?? string.Empty;
        member.MiddleName = string.IsNullOrWhiteSpace(member.MiddleName) ? null : member.MiddleName.Trim();
        if (member.LastName.Length == 0) throw new ArgumentException("Фамилия обязательна.", nameof(member));
        if (member.FirstName.Length == 0) throw new ArgumentException("Имя обязательно.", nameof(member));
    }

    private static void ValidateId(int id)
    {
        if (id <= 0) throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор должен быть положительным.");
    }

    private static bool IsDatabaseException(Exception exception) => exception is SqliteException or IOException or UnauthorizedAccessException;
}

public sealed class CommissionMemberServiceException : Exception
{
    public CommissionMemberServiceException(string message) : base(message) { }
    public CommissionMemberServiceException(string message, Exception innerException) : base(message, innerException) { }
}
