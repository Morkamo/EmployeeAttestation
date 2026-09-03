namespace EmployeeAttestation.Extra;

public static class AppDataPaths
{
    public static string ApplicationDataDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EmployeeAttestation");

    public static string ConfigPath => Path.Combine(ApplicationDataDirectory, "config.json");

    public static string DatabasesDirectory => Path.Combine(ApplicationDataDirectory, "databases");

    public static string InstallDatabasesDirectory => Path.Combine(AppContext.BaseDirectory, "databases");
}
