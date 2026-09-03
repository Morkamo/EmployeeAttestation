using EmployeeAttestation.Extra;
using Microsoft.Data.Sqlite;

namespace EmployeeAttestation.Data;

public sealed class DatabaseManager
{
    private const string DefaultDatabaseFileName = "attestation.db";
    private readonly AppConfigManager configManager;
    private readonly object syncRoot = new();
    private string selectedDatabaseFileName;

    public DatabaseManager(AppConfigManager configManager)
    {
        this.configManager = configManager ?? throw new ArgumentNullException(nameof(configManager));
        DatabasesDirectory = AppDataPaths.DatabasesDirectory;
        selectedDatabaseFileName = NormalizeDatabaseFileName(configManager.GetSelectedDatabase())
            ?? DefaultDatabaseFileName;
    }

    public string DatabasesDirectory { get; }

    public string SelectedDatabaseFileName
    {
        get
        {
            lock (syncRoot)
            {
                return selectedDatabaseFileName;
            }
        }
    }

    public void Initialize() => EnsureCurrentDatabase();

    public IReadOnlyList<string> GetAvailableDatabases()
    {
        EnsureDatabasesDirectory();

        try
        {
            return Directory
                .EnumerateFiles(DatabasesDirectory, "*.db", SearchOption.TopDirectoryOnly)
                .Where(DatabaseInitializer.IsEmployeeAttestationDatabase)
                .Select(Path.GetFileName)
                .Where(fileName => !string.IsNullOrEmpty(fileName))
                .Cast<string>()
                .OrderBy(fileName => fileName, StringComparer.CurrentCultureIgnoreCase)
                .ToArray();
        }
        catch (IOException)
        {
            return Array.Empty<string>();
        }
        catch (UnauthorizedAccessException)
        {
            return Array.Empty<string>();
        }
    }

    public string EnsureCurrentDatabase()
    {
        lock (syncRoot)
        {
            EnsureDatabasesDirectory();
            string selectedPath = GetDatabasePath(selectedDatabaseFileName);
            if (DatabaseInitializer.IsEmployeeAttestationDatabase(selectedPath))
            {
                DatabaseInitializer.InitializeDatabase(selectedPath);
                return selectedDatabaseFileName;
            }

            string? firstAvailableDatabase = GetAvailableDatabases().FirstOrDefault();
            if (firstAvailableDatabase is not null)
            {
                DatabaseInitializer.InitializeDatabase(GetDatabasePath(firstAvailableDatabase));
                SetSelectedDatabaseCore(firstAvailableDatabase);
                return selectedDatabaseFileName;
            }

            string newDatabaseFileName = GetAvailableDefaultDatabaseFileName();
            DatabaseInitializer.InitializeDatabase(GetDatabasePath(newDatabaseFileName));
            SetSelectedDatabaseCore(newDatabaseFileName);
            return selectedDatabaseFileName;
        }
    }

    public bool TrySelectDatabase(string databaseFileName)
    {
        string? normalizedFileName = NormalizeDatabaseFileName(databaseFileName);
        if (normalizedFileName is null)
        {
            return false;
        }

        lock (syncRoot)
        {
            string databasePath = GetDatabasePath(normalizedFileName);
            if (!DatabaseInitializer.IsEmployeeAttestationDatabase(databasePath))
            {
                return false;
            }

            DatabaseInitializer.InitializeDatabase(databasePath);
            SetSelectedDatabaseCore(normalizedFileName);
            return true;
        }
    }

    public bool SelectedDatabaseExists()
    {
        lock (syncRoot)
        {
            return File.Exists(GetDatabasePath(selectedDatabaseFileName));
        }
    }

    public string GetSelectedDatabasePath()
    {
        lock (syncRoot)
        {
            return GetDatabasePath(selectedDatabaseFileName);
        }
    }

    public SqliteConnection CreateConnection()
    {
        string databaseFileName = EnsureCurrentDatabase();
        SqliteConnectionStringBuilder connectionString = new()
        {
            DataSource = GetDatabasePath(databaseFileName),
            Mode = SqliteOpenMode.ReadWrite,
            Cache = SqliteCacheMode.Private,
            Pooling = false,
            ForeignKeys = true
        };
        return new SqliteConnection(connectionString.ToString());
    }

    private void SetSelectedDatabaseCore(string databaseFileName)
    {
        selectedDatabaseFileName = databaseFileName;
        configManager.SetSelectedDatabase(databaseFileName);
    }

    private void EnsureDatabasesDirectory()
    {
        Directory.CreateDirectory(DatabasesDirectory);
        CopySeedDatabasesIfNeeded();
    }

    private void CopySeedDatabasesIfNeeded()
    {
        try
        {
            bool hasUserDatabases = Directory.EnumerateFiles(DatabasesDirectory, "*.db", SearchOption.TopDirectoryOnly).Any();
            if (hasUserDatabases || !Directory.Exists(AppDataPaths.InstallDatabasesDirectory))
            {
                return;
            }

            foreach (string sourcePath in Directory.EnumerateFiles(AppDataPaths.InstallDatabasesDirectory, "*.db", SearchOption.TopDirectoryOnly))
            {
                string destinationPath = Path.Combine(DatabasesDirectory, Path.GetFileName(sourcePath));
                if (!File.Exists(destinationPath))
                {
                    File.Copy(sourcePath, destinationPath, overwrite: false);
                }
            }
        }
        catch (IOException)
        {
            // If copying seed databases fails, the app can still create a new empty database.
        }
        catch (UnauthorizedAccessException)
        {
            // If copying seed databases fails, the app can still create a new empty database.
        }
    }

    private string GetAvailableDefaultDatabaseFileName()
    {
        string defaultPath = GetDatabasePath(DefaultDatabaseFileName);
        if (!File.Exists(defaultPath))
        {
            return DefaultDatabaseFileName;
        }

        for (int index = 2; ; index++)
        {
            string candidate = $"attestation_{index}.db";
            if (!File.Exists(GetDatabasePath(candidate)))
            {
                return candidate;
            }
        }
    }

    private string GetDatabasePath(string databaseFileName) => Path.Combine(DatabasesDirectory, databaseFileName);

    private static string? NormalizeDatabaseFileName(string? databaseFileName)
    {
        if (string.IsNullOrWhiteSpace(databaseFileName)
            || !string.Equals(Path.GetFileName(databaseFileName), databaseFileName, StringComparison.Ordinal)
            || !string.Equals(Path.GetExtension(databaseFileName), ".db", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return databaseFileName;
    }
}
