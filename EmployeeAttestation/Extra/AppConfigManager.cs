using System.Text.Json;

namespace EmployeeAttestation.Extra;

public sealed class AppConfigManager
{
    private const string DefaultDatabaseFileName = "attestation.db";
    private readonly object syncRoot = new();
    private AppConfig config;

    public AppConfigManager()
    {
        ConfigPath = Path.Combine(AppContext.BaseDirectory, "config.json");
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
        if (!File.Exists(ConfigPath))
        {
            AppConfig defaultConfig = CreateDefaultConfig();
            SaveConfig(defaultConfig);
            return defaultConfig;
        }

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

    private sealed class AppConfig
    {
        public string SelectedDatabase { get; set; } = DefaultDatabaseFileName;
    }
}
