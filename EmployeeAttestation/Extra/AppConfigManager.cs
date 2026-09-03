using System.Text.Json;

namespace EmployeeAttestation.Extra;

public sealed class AppConfigManager
{
    private const string DefaultDatabaseFileName = "attestation.db";
    private readonly object syncRoot = new();
    private AppConfig config;

    public AppConfigManager()
    {
        ConfigPath = AppDataPaths.ConfigPath;
        config = LoadConfig();
    }

    public string ConfigPath { get; }

    public string GetSelectedDatabase()
    {
        lock (syncRoot)
        {
            return config.SelectedDatabase;
        }
    }

    public void SetSelectedDatabase(string databaseFileName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseFileName);

        lock (syncRoot)
        {
            config.SelectedDatabase = databaseFileName;
            SaveConfig(config);
        }
    }

    public void Save()
    {
        lock (syncRoot)
        {
            SaveConfig(config);
        }
    }

    private AppConfig LoadConfig()
    {
        EnsureApplicationDataDirectory();

        if (!File.Exists(ConfigPath))
        {
            TryMigrateLegacyConfig();

            if (File.Exists(ConfigPath))
            {
                return LoadConfigFromFile();
            }

            AppConfig defaultConfig = CreateDefaultConfig();
            SaveConfig(defaultConfig);
            return defaultConfig;
        }

        return LoadConfigFromFile();
    }

    private AppConfig LoadConfigFromFile()
    {
        try
        {
            string json = File.ReadAllText(ConfigPath);
            AppConfig? loadedConfig = JsonSerializer.Deserialize<AppConfig>(json);
            if (loadedConfig is null || string.IsNullOrWhiteSpace(loadedConfig.SelectedDatabase))
            {
                throw new JsonException("Configuration does not contain SelectedDatabase.");
            }

            return loadedConfig;
        }
        catch (JsonException)
        {
            return ResetConfig();
        }
        catch (IOException)
        {
            return CreateDefaultConfig();
        }
        catch (UnauthorizedAccessException)
        {
            return CreateDefaultConfig();
        }
    }

    private AppConfig ResetConfig()
    {
        AppConfig defaultConfig = CreateDefaultConfig();
        SaveConfig(defaultConfig);
        return defaultConfig;
    }

    private void SaveConfig(AppConfig configToSave)
    {
        try
        {
            EnsureApplicationDataDirectory();
            string json = JsonSerializer.Serialize(configToSave, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
        catch (IOException)
        {
            // Configuration persistence must not prevent the application from starting.
        }
        catch (UnauthorizedAccessException)
        {
            // Configuration persistence must not prevent the application from starting.
        }
    }

    private static AppConfig CreateDefaultConfig() => new() { SelectedDatabase = DefaultDatabaseFileName };

    private static void EnsureApplicationDataDirectory()
    {
        Directory.CreateDirectory(AppDataPaths.ApplicationDataDirectory);
    }

    private void TryMigrateLegacyConfig()
    {
        string legacyConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");
        if (!File.Exists(legacyConfigPath))
        {
            return;
        }

        try
        {
            File.Copy(legacyConfigPath, ConfigPath, overwrite: false);
        }
        catch (IOException)
        {
            // Ignore migration failures and fall back to defaults.
        }
        catch (UnauthorizedAccessException)
        {
            // Ignore migration failures and fall back to defaults.
        }
    }

    private sealed class AppConfig
    {
        public string SelectedDatabase { get; set; } = DefaultDatabaseFileName;
    }
}
