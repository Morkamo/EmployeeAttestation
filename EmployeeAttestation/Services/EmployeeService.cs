using EmployeeAttestation.Data;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class EmployeeService
{
    private readonly DatabaseManager databaseManager;

    public EmployeeService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<EmployeeListItem> GetAll(bool? isArchived = false) => QueryEmployees(null, isArchived);

    public List<EmployeeListItem> Search(string query, bool? isArchived = false)
    {
        return string.IsNullOrWhiteSpace(query)
            ? GetAll(isArchived)
            : QueryEmployees(query.Trim(), isArchived);
    }

    public Employee? GetById(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, last_name, first_name, middle_name, department_id, position_id, is_manager, is_archived
                FROM employees
                WHERE id = $id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadEmployee(reader) : null;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EmployeeServiceException("Не удалось загрузить сотрудника.", exception);
        }
    }

    public int Create(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        NormalizeAndValidate(employee);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO employees
                    (last_name, first_name, middle_name, department_id, position_id, is_manager, is_archived)
                VALUES
                    ($lastName, $firstName, $middleName, $departmentId, $positionId, $isManager, $isArchived);
                SELECT last_insert_rowid();
                """;
            AddEmployeeParameters(command, employee);
            long id = (long)(command.ExecuteScalar()
                ?? throw new EmployeeServiceException("Не удалось получить идентификатор сотрудника."));
            employee.Id = checked((int)id);
            return employee.Id;
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new EmployeeServiceException(
                "Выбранное подразделение или должность больше не существует.",
                exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EmployeeServiceException("Не удалось сохранить сотрудника.", exception);
        }
    }

    public void Update(Employee employee)
    {
        ArgumentNullException.ThrowIfNull(employee);
        ValidateId(employee.Id);
        NormalizeAndValidate(employee);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE employees
                SET last_name = $lastName,
                    first_name = $firstName,
                    middle_name = $middleName,
                    department_id = $departmentId,
                    position_id = $positionId,
                    is_manager = $isManager,
                    is_archived = $isArchived
                WHERE id = $id;
                """;
            AddEmployeeParameters(command, employee);
            command.Parameters.AddWithValue("$id", employee.Id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new EmployeeServiceException("Сотрудник не найден.");
            }
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new EmployeeServiceException(
                "Выбранное подразделение или должность больше не существует.",
                exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EmployeeServiceException("Не удалось сохранить сотрудника.", exception);
        }
    }

    public void Archive(int id) => SetArchived(id, true);

    public void Restore(int id) => SetArchived(id, false);

    private List<EmployeeListItem> QueryEmployees(string? query, bool? isArchived)
    {
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT e.id,
                       e.last_name,
                       e.first_name,
                       e.middle_name,
                       d.name,
                       p.name,
                       e.is_manager,
                       e.is_archived
                FROM employees AS e
                INNER JOIN departments AS d ON d.id = e.department_id
                INNER JOIN positions AS p ON p.id = e.position_id
                WHERE ($isArchived IS NULL OR e.is_archived = $isArchived)
                  AND ($query IS NULL
                       OR e.last_name LIKE $query
                       OR e.first_name LIKE $query
                       OR e.middle_name LIKE $query
                       OR d.name LIKE $query
                       OR p.name LIKE $query)
                ORDER BY e.last_name, e.first_name, e.middle_name;
                """;
            command.Parameters.AddWithValue(
                "$isArchived",
                isArchived.HasValue ? (isArchived.Value ? 1 : 0) : DBNull.Value);
            command.Parameters.AddWithValue("$query", query is null ? DBNull.Value : $"%{query}%");
            using SqliteDataReader reader = command.ExecuteReader();
            List<EmployeeListItem> employees = [];
            while (reader.Read())
            {
                employees.Add(new EmployeeListItem
                {
                    Id = reader.GetInt32(0),
                    FullName = BuildFullName(
                        reader.GetString(1),
                        reader.GetString(2),
                        reader.IsDBNull(3) ? null : reader.GetString(3)),
                    DepartmentName = reader.GetString(4),
                    PositionName = reader.GetString(5),
                    IsManager = reader.GetInt32(6) == 1,
                    IsArchived = reader.GetInt32(7) == 1
                });
            }
            return employees;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EmployeeServiceException("Не удалось загрузить сотрудников.", exception);
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
            command.CommandText = "UPDATE employees SET is_archived = $isArchived WHERE id = $id;";
            command.Parameters.AddWithValue("$isArchived", isArchived ? 1 : 0);
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new EmployeeServiceException("Сотрудник не найден.");
            }
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new EmployeeServiceException(
                isArchived ? "Не удалось архивировать сотрудника." : "Не удалось восстановить сотрудника.",
                exception);
        }
    }

    private static Employee ReadEmployee(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        LastName = reader.GetString(1),
        FirstName = reader.GetString(2),
        MiddleName = reader.IsDBNull(3) ? null : reader.GetString(3),
        DepartmentId = reader.GetInt32(4),
        PositionId = reader.GetInt32(5),
        IsManager = reader.GetInt32(6) == 1,
        IsArchived = reader.GetInt32(7) == 1
    };

    private static void AddEmployeeParameters(SqliteCommand command, Employee employee)
    {
        command.Parameters.AddWithValue("$lastName", employee.LastName);
        command.Parameters.AddWithValue("$firstName", employee.FirstName);
        command.Parameters.AddWithValue("$middleName", (object?)employee.MiddleName ?? DBNull.Value);
        command.Parameters.AddWithValue("$departmentId", employee.DepartmentId);
        command.Parameters.AddWithValue("$positionId", employee.PositionId);
        command.Parameters.AddWithValue("$isManager", employee.IsManager ? 1 : 0);
        command.Parameters.AddWithValue("$isArchived", employee.IsArchived ? 1 : 0);
    }

    private static void NormalizeAndValidate(Employee employee)
    {
        employee.LastName = employee.LastName?.Trim() ?? string.Empty;
        employee.FirstName = employee.FirstName?.Trim() ?? string.Empty;
        employee.MiddleName = string.IsNullOrWhiteSpace(employee.MiddleName) ? null : employee.MiddleName.Trim();
        if (employee.LastName.Length == 0)
        {
            throw new ArgumentException("Фамилия сотрудника обязательна.", nameof(employee));
        }
        if (employee.FirstName.Length == 0)
        {
            throw new ArgumentException("Имя сотрудника обязательно.", nameof(employee));
        }
        if (employee.DepartmentId <= 0)
        {
            throw new ArgumentException("Выберите подразделение.", nameof(employee));
        }
        if (employee.PositionId <= 0)
        {
            throw new ArgumentException("Выберите должность.", nameof(employee));
        }
    }

    private static string BuildFullName(string lastName, string firstName, string? middleName) => string.Join(
        " ",
        new[] { lastName, firstName, middleName }.Where(value => !string.IsNullOrWhiteSpace(value)));

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор сотрудника должен быть положительным.");
        }
    }

    private static bool IsDatabaseException(Exception exception) => exception is SqliteException
        or IOException
        or UnauthorizedAccessException;
}

public sealed class EmployeeServiceException : Exception
{
    public EmployeeServiceException(string message) : base(message) { }

    public EmployeeServiceException(string message, Exception innerException) : base(message, innerException) { }
}
