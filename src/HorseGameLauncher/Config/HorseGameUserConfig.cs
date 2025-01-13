using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Serilog;

namespace HorseGameLauncher.Config;

public class HorseGameUserConfig
{
    [JsonIgnore]
    public static HorseGameUserConfig Instance { get; private set; } = new();

    [JsonPropertyName("wine_prefix_dir")]
    public string WinePrefixDir { get; set; }

    public static void Load()
    {
        string configPath = GetConfigPath();
        if (File.Exists(configPath))
        {
            string serializedConfig = File.ReadAllText(configPath);
            HorseGameUserConfig? config = JsonSerializer.Deserialize<HorseGameUserConfig>(serializedConfig);

            Instance = config ?? new HorseGameUserConfig();
        }

        Log.Information("User config loaded");
    }

    public static void Save()
    {
        string configPath = GetConfigPath();
        string serializedConfig = JsonSerializer.Serialize(Instance);

        File.WriteAllText(configPath, serializedConfig);

        Log.Information("User config saved");
    }

    private static string GetConfigPath()
    {
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string configDir = Path.Combine(appDataPath, HorseGameLauncherConfig.ApplicationName);

        Directory.CreateDirectory(configDir);

        return Path.Combine(configDir, "config.json");
    }
}