using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Data;

public static class DatabaseInitializer
{
    public const string ApplicationName = "EmployeeAttestation";
    public const string SchemaVersion = "2";
    private const string PreviousSchemaVersion = "1";

    public static void InitializeDatabase(string databasePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databasePath);
        string? directory = Path.GetDirectoryName(databasePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using SqliteConnection connection = CreateConnection(databasePath, SqliteOpenMode.ReadWriteCreate);
        connection.Open();
        using SqliteTransaction transaction = connection.BeginTransaction();

        using SqliteCommand createTableCommand = connection.CreateCommand();
        createTableCommand.Transaction = transaction;
        createTableCommand.CommandText =
            """
            CREATE TABLE IF NOT EXISTS app_metadata (
                key TEXT PRIMARY KEY,
                value TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS departments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                code TEXT NOT NULL UNIQUE,
                name TEXT NOT NULL,
                document_name TEXT NULL
            );

            CREATE TABLE IF NOT EXISTS positions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL UNIQUE,
                is_managerial INTEGER NOT NULL DEFAULT 0
                    CHECK (is_managerial IN (0, 1))
            );

            CREATE TABLE IF NOT EXISTS employees (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                last_name TEXT NOT NULL,
                first_name TEXT NOT NULL,
                middle_name TEXT NULL,
                department_id INTEGER NOT NULL,
                position_id INTEGER NOT NULL,
                is_manager INTEGER NOT NULL DEFAULT 0
                    CHECK (is_manager IN (0, 1)),
                is_archived INTEGER NOT NULL DEFAULT 0
                    CHECK (is_archived IN (0, 1)),
                FOREIGN KEY (department_id) REFERENCES departments(id) ON DELETE RESTRICT,
                FOREIGN KEY (position_id) REFERENCES positions(id) ON DELETE RESTRICT
            );

            CREATE TABLE IF NOT EXISTS commission_members (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                last_name TEXT NOT NULL,
                first_name TEXT NOT NULL,
                middle_name TEXT NULL,
                is_archived INTEGER NOT NULL DEFAULT 0
                    CHECK (is_archived IN (0, 1))
            );

            CREATE TABLE IF NOT EXISTS commissions (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                name TEXT NOT NULL,
                description TEXT NULL,
                is_archived INTEGER NOT NULL DEFAULT 0
                    CHECK (is_archived IN (0, 1))
            );

            CREATE TABLE IF NOT EXISTS commission_composition (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                commission_id INTEGER NOT NULL,
                commission_member_id INTEGER NOT NULL,
                role TEXT NOT NULL,
                sort_order INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (commission_id) REFERENCES commissions(id) ON DELETE RESTRICT,
                FOREIGN KEY (commission_member_id) REFERENCES commission_members(id) ON DELETE RESTRICT,
                UNIQUE (commission_id, commission_member_id)
            );

            CREATE TABLE IF NOT EXISTS evaluation_criteria (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                code TEXT NOT NULL UNIQUE,
                category TEXT NOT NULL,
                name TEXT NOT NULL,
                minimum_score INTEGER NOT NULL DEFAULT 2,
                maximum_score INTEGER NOT NULL DEFAULT 5,
                managers_only INTEGER NOT NULL DEFAULT 0
                    CHECK (managers_only IN (0, 1)),
                sort_order INTEGER NOT NULL,
                is_active INTEGER NOT NULL DEFAULT 1
                    CHECK (is_active IN (0, 1))
            );

            CREATE TABLE IF NOT EXISTS attestations (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                employee_id INTEGER NOT NULL,
                commission_id INTEGER NOT NULL,
                attestation_date TEXT NULL,
                status TEXT NOT NULL,
                evaluate_managerial INTEGER NOT NULL DEFAULT 0
                    CHECK (evaluate_managerial IN (0, 1)),
                professional_average REAL NULL,
                personal_average REAL NULL,
                managerial_average REAL NULL,
                overall_average REAL NULL,
                decision TEXT NULL,
                recommendations TEXT NULL,
                commission_members_count INTEGER NULL,
                present_members_count INTEGER NULL,
                votes_for INTEGER NULL,
                votes_against INTEGER NULL,
                votes_abstained INTEGER NULL,
                created_at TEXT NOT NULL,
                completed_at TEXT NULL,
                FOREIGN KEY (employee_id) REFERENCES employees(id) ON DELETE RESTRICT,
                FOREIGN KEY (commission_id) REFERENCES commissions(id) ON DELETE RESTRICT
            );

            CREATE TABLE IF NOT EXISTS attestation_scores (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                attestation_id INTEGER NOT NULL,
                commission_member_id INTEGER NOT NULL,
                criterion_id INTEGER NOT NULL,
                score INTEGER NOT NULL CHECK (score BETWEEN 1 AND 5),
                FOREIGN KEY (attestation_id) REFERENCES attestations(id) ON DELETE RESTRICT,
                FOREIGN KEY (commission_member_id) REFERENCES commission_members(id) ON DELETE RESTRICT,
                FOREIGN KEY (criterion_id) REFERENCES evaluation_criteria(id) ON DELETE RESTRICT,
                UNIQUE (attestation_id, commission_member_id, criterion_id)
            );

            CREATE TABLE IF NOT EXISTS attestation_votes (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                attestation_id INTEGER NOT NULL,
                commission_member_id INTEGER NOT NULL,
                vote TEXT NOT NULL CHECK (vote IN ('For', 'Against', 'Abstained')),
                FOREIGN KEY (attestation_id) REFERENCES attestations(id) ON DELETE RESTRICT,
                FOREIGN KEY (commission_member_id) REFERENCES commission_members(id) ON DELETE RESTRICT,
                UNIQUE (attestation_id, commission_member_id)
            );

            CREATE INDEX IF NOT EXISTS idx_employees_department_id
                ON employees(department_id);
            CREATE INDEX IF NOT EXISTS idx_employees_position_id
                ON employees(position_id);
            CREATE INDEX IF NOT EXISTS idx_attestations_employee_id
                ON attestations(employee_id);
            CREATE INDEX IF NOT EXISTS idx_attestations_commission_id
                ON attestations(commission_id);
            CREATE INDEX IF NOT EXISTS idx_attestations_status
                ON attestations(status);
            CREATE INDEX IF NOT EXISTS idx_attestations_attestation_date
                ON attestations(attestation_date);
            CREATE INDEX IF NOT EXISTS idx_commission_composition_commission_id
                ON commission_composition(commission_id);
            CREATE INDEX IF NOT EXISTS idx_attestation_scores_attestation_id
                ON attestation_scores(attestation_id);
            CREATE INDEX IF NOT EXISTS idx_attestation_scores_commission_member_id
                ON attestation_scores(commission_member_id);
            CREATE INDEX IF NOT EXISTS idx_attestation_scores_criterion_id
                ON attestation_scores(criterion_id);
            CREATE INDEX IF NOT EXISTS idx_attestation_votes_attestation_id
                ON attestation_votes(attestation_id);
            """;
        createTableCommand.ExecuteNonQuery();

        MigrateToVersion2(connection, transaction);
        WriteMetadata(connection, transaction, "application", ApplicationName);
        WriteMetadata(connection, transaction, "schema_version", SchemaVersion);
        SeedEvaluationCriteria(connection, transaction);
        transaction.Commit();
    }

    public static bool IsEmployeeAttestationDatabase(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath) || !File.Exists(databasePath))
        {
            return false;
        }

        try
        {
            using SqliteConnection connection = CreateConnection(databasePath, SqliteOpenMode.ReadOnly);
            connection.Open();

            using SqliteCommand tableCommand = connection.CreateCommand();
            tableCommand.CommandText =
                "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'app_metadata';";
            if (Convert.ToInt32(tableCommand.ExecuteScalar()) != 1)
            {
                return false;
            }

            string? application = ReadMetadata(connection, "application");
            string? schemaVersion = ReadMetadata(connection, "schema_version");
            return string.Equals(application, ApplicationName, StringComparison.Ordinal)
                && (string.Equals(schemaVersion, SchemaVersion, StringComparison.Ordinal)
                    || string.Equals(schemaVersion, PreviousSchemaVersion, StringComparison.Ordinal));
        }
        catch (SqliteException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    private static SqliteConnection CreateConnection(string databasePath, SqliteOpenMode mode)
    {
        SqliteConnectionStringBuilder connectionString = new()
        {
            DataSource = databasePath,
            Mode = mode,
            Cache = SqliteCacheMode.Private,
            Pooling = false,
            ForeignKeys = true
        };
        return new SqliteConnection(connectionString.ToString());
    }

    private static void SeedEvaluationCriteria(
        SqliteConnection connection,
        SqliteTransaction transaction)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            INSERT OR IGNORE INTO evaluation_criteria
                (code, category, name, minimum_score, maximum_score, managers_only, sort_order, is_active)
            VALUES
                ('PROFESSIONAL_KNOWLEDGE', 'Professional', 'Профессиональные знания', 2, 5, 0, 1, 1),
                ('PROFESSIONAL_SKILLS', 'Professional', 'Профессиональные умения и навыки', 2, 5, 0, 2, 1),
                ('PROFESSIONAL_EXPERIENCE', 'Professional', 'Профессиональный опыт', 2, 5, 0, 3, 1),
                ('WORK_PROCESS_ORGANIZATION', 'Professional', 'Организация трудового процесса', 2, 5, 0, 4, 1),
                ('WORK_CAPACITY', 'Personal', 'Работоспособность, интенсивность труда', 2, 5, 0, 1, 1),
                ('INDEPENDENCE', 'Personal', 'Самостоятельность решений и действий, способность к самокритике', 2, 5, 0, 2, 1),
                ('BEHAVIOR_ETHICS', 'Personal', 'Этика поведения, стиль общения', 2, 5, 0, 3, 1),
                ('SOCIAL_INTERACTION', 'Personal', 'Социальное взаимодействие', 2, 5, 0, 4, 1),
                ('STRATEGIC_OPERATIONAL_QUALITIES', 'Managerial', 'Стратегические и оперативные качества, эффективность деятельности', 2, 5, 1, 1, 1),
                ('MANAGEMENT_EXPERIENCE', 'Managerial', 'Опыт руководства подчиненными, результативность', 2, 5, 1, 2, 1),
                ('LEADERSHIP', 'Managerial', 'Лидерство', 2, 5, 1, 3, 1),
                ('DEMANDINGNESS', 'Managerial', 'Требовательность', 2, 5, 1, 4, 1);
            """;
        command.ExecuteNonQuery();
    }

    private static void MigrateToVersion2(
        SqliteConnection connection,
        SqliteTransaction transaction)
    {
        using (SqliteCommand command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText =
                """
                CREATE TABLE IF NOT EXISTS attestation_criteria (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    attestation_id INTEGER NOT NULL,
                    criterion_id INTEGER NULL,
                    criterion_code TEXT NOT NULL,
                    criterion_name TEXT NOT NULL,
                    category TEXT NOT NULL,
                    minimum_score INTEGER NOT NULL,
                    maximum_score INTEGER NOT NULL,
                    managers_only INTEGER NOT NULL DEFAULT 0
                        CHECK (managers_only IN (0, 1)),
                    sort_order INTEGER NOT NULL DEFAULT 0,
                    FOREIGN KEY (attestation_id) REFERENCES attestations(id) ON DELETE RESTRICT,
                    FOREIGN KEY (criterion_id) REFERENCES evaluation_criteria(id) ON DELETE SET NULL,
                    UNIQUE (attestation_id, criterion_code)
                );

                CREATE TABLE IF NOT EXISTS attestation_commission_members (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    attestation_id INTEGER NOT NULL,
                    commission_member_id INTEGER NULL,
                    member_full_name TEXT NOT NULL,
                    role TEXT NOT NULL,
                    sort_order INTEGER NOT NULL DEFAULT 0,
                    is_present INTEGER NOT NULL DEFAULT 1
                        CHECK (is_present IN (0, 1)),
                    FOREIGN KEY (attestation_id) REFERENCES attestations(id) ON DELETE RESTRICT,
                    FOREIGN KEY (commission_member_id) REFERENCES commission_members(id) ON DELETE SET NULL,
                    UNIQUE (attestation_id, commission_member_id)
                );

                CREATE INDEX IF NOT EXISTS idx_attestation_criteria_attestation_id
                    ON attestation_criteria(attestation_id);
                CREATE INDEX IF NOT EXISTS idx_attestation_commission_members_attestation_id
                    ON attestation_commission_members(attestation_id);
                """;
            command.ExecuteNonQuery();
        }

        AddColumnIfMissing(
            connection,
            transaction,
            "attestation_scores",
            "attestation_criterion_id",
            "INTEGER NULL REFERENCES attestation_criteria(id) ON DELETE RESTRICT");
        AddColumnIfMissing(
            connection,
            transaction,
            "attestation_scores",
            "attestation_commission_member_id",
            "INTEGER NULL REFERENCES attestation_commission_members(id) ON DELETE RESTRICT");
        AddColumnIfMissing(
            connection,
            transaction,
            "attestation_votes",
            "attestation_commission_member_id",
            "INTEGER NULL REFERENCES attestation_commission_members(id) ON DELETE RESTRICT");

        using SqliteCommand indexCommand = connection.CreateCommand();
        indexCommand.Transaction = transaction;
        indexCommand.CommandText =
            """
            CREATE UNIQUE INDEX IF NOT EXISTS ux_attestation_scores_snapshot
                ON attestation_scores(
                    attestation_id,
                    attestation_commission_member_id,
                    attestation_criterion_id)
                WHERE attestation_commission_member_id IS NOT NULL
                  AND attestation_criterion_id IS NOT NULL;
            CREATE UNIQUE INDEX IF NOT EXISTS ux_attestation_votes_snapshot
                ON attestation_votes(attestation_id, attestation_commission_member_id)
                WHERE attestation_commission_member_id IS NOT NULL;
            CREATE INDEX IF NOT EXISTS idx_attestation_scores_attestation_criterion_id
                ON attestation_scores(attestation_criterion_id);
            CREATE INDEX IF NOT EXISTS idx_attestation_votes_snapshot_member_id
                ON attestation_votes(attestation_commission_member_id);
            """;
        indexCommand.ExecuteNonQuery();
    }

    private static void AddColumnIfMissing(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string tableName,
        string columnName,
        string definition)
    {
        using (SqliteCommand checkCommand = connection.CreateCommand())
        {
            checkCommand.Transaction = transaction;
            checkCommand.CommandText = $"PRAGMA table_info({tableName});";
            using SqliteDataReader reader = checkCommand.ExecuteReader();
            while (reader.Read())
            {
                if (string.Equals(reader.GetString(1), columnName, StringComparison.OrdinalIgnoreCase))
                {
                    return;
                }
            }
        }

        using SqliteCommand alterCommand = connection.CreateCommand();
        alterCommand.Transaction = transaction;
        alterCommand.CommandText = $"ALTER TABLE {tableName} ADD COLUMN {columnName} {definition};";
        alterCommand.ExecuteNonQuery();
    }

    private static void WriteMetadata(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string key,
        string value)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText =
            """
            INSERT INTO app_metadata (key, value)
            VALUES ($key, $value)
            ON CONFLICT(key) DO UPDATE SET value = excluded.value;
            """;
        command.Parameters.AddWithValue("$key", key);
        command.Parameters.AddWithValue("$value", value);
        command.ExecuteNonQuery();
    }

    private static string? ReadMetadata(SqliteConnection connection, string key)
    {
        using SqliteCommand command = connection.CreateCommand();
        command.CommandText = "SELECT value FROM app_metadata WHERE key = $key LIMIT 1;";
        command.Parameters.AddWithValue("$key", key);
        return command.ExecuteScalar() as string;
    }
}
