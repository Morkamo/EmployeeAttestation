using EmployeeAttestation.Data;
using EmployeeAttestation.Models;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Services;

public sealed class PositionService
{
    private const int SqliteConstraintUnique = 2067;
    private readonly DatabaseManager databaseManager;

    public PositionService(DatabaseManager databaseManager)
    {
        this.databaseManager = databaseManager ?? throw new ArgumentNullException(nameof(databaseManager));
    }

    public List<Position> GetAll()
    {
        const string sql =
            """
            SELECT id, name, is_managerial
            FROM positions
            ORDER BY name;
            """;
        return QueryPositions(sql, null);
    }

    public List<Position> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return GetAll();
        }

        const string sql =
            """
            SELECT id, name, is_managerial
            FROM positions
            WHERE name LIKE $query
            ORDER BY name;
            """;
        return QueryPositions(sql, command => command.Parameters.AddWithValue("$query", $"%{query.Trim()}%"));
    }

    public Position? GetById(int id)
    {
        ValidateId(id);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                SELECT id, name, is_managerial
                FROM positions
                WHERE id = $id
                LIMIT 1;
                """;
            command.Parameters.AddWithValue("$id", id);
            using SqliteDataReader reader = command.ExecuteReader();
            return reader.Read() ? ReadPosition(reader) : null;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new PositionServiceException("Не удалось загрузить должность.", exception);
        }
    }

    public int Create(Position position)
    {
        ArgumentNullException.ThrowIfNull(position);
        NormalizeAndValidate(position);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                INSERT INTO positions (name, is_managerial)
                VALUES ($name, $isManagerial);
                SELECT last_insert_rowid();
                """;
            AddPositionParameters(command, position);
            long id = (long)(command.ExecuteScalar()
                ?? throw new PositionServiceException("Не удалось получить идентификатор должности."));
            position.Id = checked((int)id);
            return position.Id;
        }
        catch (SqliteException exception) when (exception.SqliteExtendedErrorCode == SqliteConstraintUnique)
        {
            throw new PositionServiceException("Должность с таким наименованием уже существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new PositionServiceException("Не удалось сохранить должность.", exception);
        }
    }

    public void Update(Position position)
    {
        ArgumentNullException.ThrowIfNull(position);
        ValidateId(position.Id);
        NormalizeAndValidate(position);
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText =
                """
                UPDATE positions
                SET name = $name,
                    is_managerial = $isManagerial
                WHERE id = $id;
                """;
            AddPositionParameters(command, position);
            command.Parameters.AddWithValue("$id", position.Id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new PositionServiceException("Должность не найдена.");
            }
        }
        catch (SqliteException exception) when (exception.SqliteExtendedErrorCode == SqliteConstraintUnique)
        {
            throw new PositionServiceException("Должность с таким наименованием уже существует.", exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new PositionServiceException("Не удалось сохранить должность.", exception);
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
            command.CommandText = "DELETE FROM positions WHERE id = $id;";
            command.Parameters.AddWithValue("$id", id);
            if (command.ExecuteNonQuery() == 0)
            {
                throw new PositionServiceException("Должность не найдена.");
            }
        }
        catch (SqliteException exception) when (exception.SqliteErrorCode == 19)
        {
            throw new PositionServiceException(
                "Невозможно удалить должность, поскольку она используется сотрудниками.",
                exception);
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new PositionServiceException("Не удалось удалить должность.", exception);
        }
    }

    private List<Position> QueryPositions(string sql, Action<SqliteCommand>? configureCommand)
    {
        try
        {
            using SqliteConnection connection = databaseManager.CreateConnection();
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = sql;
            configureCommand?.Invoke(command);
            using SqliteDataReader reader = command.ExecuteReader();
            List<Position> positions = [];
            while (reader.Read())
            {
                positions.Add(ReadPosition(reader));
            }
            return positions;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw new PositionServiceException("Не удалось загрузить должности.", exception);
        }
    }

    private static Position ReadPosition(SqliteDataReader reader) => new()
    {
        Id = reader.GetInt32(0),
        Name = reader.GetString(1),
        IsManagerial = reader.GetInt32(2) == 1
    };

    private static void AddPositionParameters(SqliteCommand command, Position position)
    {
        command.Parameters.AddWithValue("$name", position.Name);
        command.Parameters.AddWithValue("$isManagerial", position.IsManagerial ? 1 : 0);
    }

    private static void NormalizeAndValidate(Position position)
    {
        position.Name = position.Name?.Trim() ?? string.Empty;
        if (position.Name.Length == 0)
        {
            throw new ArgumentException("Наименование должности обязательно.", nameof(position));
        }
    }

    private static void ValidateId(int id)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), "Идентификатор должности должен быть положительным.");
        }
    }

    private static bool IsDatabaseException(Exception exception) => exception is SqliteException
        or IOException
        or UnauthorizedAccessException;
}

public sealed class PositionServiceException : Exception
{
    public PositionServiceException(string message) : base(message) { }

    public PositionServiceException(string message, Exception innerException) : base(message, innerException) { }
}
