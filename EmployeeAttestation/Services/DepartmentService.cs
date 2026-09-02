using EmployeeAttestation.Data;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class DepartmentService
{
    private const int SqliteConstraintUnique = 2067;
    private readonly DatabaseManager databaseManager;

    public DepartmentService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<Department> GetAll()
    {
        const string sql =
            """
            SELECT id, code, name, document_name
            FROM departments
            ORDER BY code, name;
            """;

        return QueryDepartments(sql, null);
    }

    public List<Department> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAll();
        }

        const string sql =
            """
            SELECT id, code, name, document_name
            FROM departments
            WHERE code LIKE $query
               OR name LIKE $query
               OR document_name LIKE $query
            ORDER BY code, name;
            """;

        return QueryDepartments(sql, command => command.Parameters.AddWithValue("$query", $"%{query.Trim()}%"));
    }

    public Department? GetById(int id)
    {
        ValidateId(id);

        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, code, name, document_name
                FROM departments
                WHERE id = $id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadDepartment(reader) : null;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new DepartmentServiceException("Не удалось загрузить подразделение.", exception);
        }
    }

    public int Create(Department department)
    {
        ArgumentNullException.ThrowIfNull(department);
        NormalizeAndValidate(department);

        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO departments (code, name, document_name)
                VALUES ($code, $name, $documentName);
                SELECT last_insert_rowid();
                """;
            AddDepartmentParameters(command, department);
            long id = (long)(command.ExecuteScalar()
                ?? throw new DepartmentServiceException("Не удалось получить идентификатор подразделения."));
            department.Id = checked((int)id);
            return department.Id;
        }
        catch (SqliteException exception) when (exception.SqliteExtendedErrorCode == SqliteConstraintUnique)
        {
            throw new DepartmentServiceException("Подразделение с таким кодом уже существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new DepartmentServiceException("Не удалось сохранить подразделение.", exception);
        }
    }

    public void Update(Department department)
    {
        ArgumentNullException.ThrowIfNull(department);
        ValidateId(department.Id);
        NormalizeAndValidate(department);

        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE departments
                SET code = $code,
                    name = $name,
                    document_name = $documentName
                WHERE id = $id;
                """;
            AddDepartmentParameters(command, department);
            command.Parameters.AddWithValue("$id", department.Id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new DepartmentServiceException("Подразделение не найдено.");
            }
        }
        catch (SqliteException exception) when (exception.SqliteExtendedErrorCode == SqliteConstraintUnique)
        {
            throw new DepartmentServiceException("Подразделение с таким кодом уже существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new DepartmentServiceException("Не удалось сохранить подразделение.", exception);
        }
    }

    public void Delete(int id)
    {
        ValidateId(id);

        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "DELETE FROM departments WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new DepartmentServiceException("Подразделение не найдено.");
            }
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new DepartmentServiceException(
                "Невозможно удалить подразделение, поскольку оно используется другими данными.",
                exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new DepartmentServiceException("Не удалось удалить подразделение.", exception);
        }
    }

    private List<Department> QueryDepartments(string sql, Action<SqliteCommand>? configureCommand)
    {
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            configureCommand?.Invoke(command);
            using SqliteDataReader reader = command.ExecuteReader();
            List<Department> departments = [];
            while (reader.Read())
            {
                departments.Add(ReadDepartment(reader));
            }

            return departments;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new DepartmentServiceException("Не удалось загрузить подразделения.", exception);
        }
    }

    private static Department ReadDepartment(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Code = reader.GetString(1),
        Name = reader.GetString(2),
        DocumentName = reader.IsDBNull(3) ? null : reader.GetString(3)
    };

    private static void AddDepartmentParameters(SqliteCommand command, Department department)
    {
        command.Parameters.AddWithValue("$code", department.Code);
        command.Parameters.AddWithValue("$name", department.Name);
        command.Parameters.AddWithValue("$documentName", (object?)department.DocumentName ?? DBNull.Value);
    }

    private static void NormalizeAndValidate(Department department)
    {
        department.Code = department.Code?.Trim() ?? string.Empty;
        department.Name = department.Name?.Trim() ?? string.Empty;
        department.DocumentName = string.IsNullOrWhiteSpace(department.DocumentName)
            ? null
            : department.DocumentName.Trim();

        if (department.Code.Length == 0)
        {
            throw new ArgumentException("Код подразделения обязателен.", nameof(department));
        }

        if (department.Name.Length == 0)
        {
            throw new ArgumentException("Наименование подразделения обязательно.", nameof(department));
        }
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор подразделения должен быть положительным.");
        }
    }

    private static bool IsDatabaseException(Exception exception) => exception is SqliteException
        or IOException
        or UnauthorizedAccessException;
}

public sealed class DepartmentServiceException : Exception
{
    public DepartmentServiceException(string message)
        : base(message)
    {
    }

    public DepartmentServiceException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
