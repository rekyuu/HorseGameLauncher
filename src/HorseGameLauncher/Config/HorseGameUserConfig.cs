using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using HorseGameLauncher.Utility;
using Serilog;

namespace HorseGameLauncher.Config;

internal class HorseGameUserConfig
{
    [JsonIgnore]
    internal static HorseGameUserConfig Instance { get; private set; } = new();

    [JsonPropertyName("data_dir")]
    public string? DataDir { get; set; }

    [JsonPropertyName("wine_version")]
    public string? WineVersion { get; set; }

    [JsonPropertyName("dxvk_version")]
    public string? DxvkVersion { get; set; }

    internal static async Task Load()
    {
        Log.Information("Loading user config");

        string configPath = PathUtility.GetConfigFilePath();
        if (File.Exists(configPath))
        {
            string serializedConfig = await File.ReadAllTextAsync(configPath);
            HorseGameUserConfig? config = JsonSerializer.Deserialize<HorseGameUserConfig>(serializedConfig);

            Instance = config ?? GetDefaultConfig();
        }
        else
        {
            Instance = GetDefaultConfig();
            await Save();
        }
    }

    internal static async Task Save()
    {
        Log.Information("Saving user config");

        string configPath = PathUtility.GetConfigFilePath();
        string serializedConfig = JsonSerializer.Serialize(Instance);

        await File.WriteAllTextAsync(configPath, serializedConfig);
    }

    internal static HorseGameUserConfig GetDefaultConfig()
    {
        Log.Information("Creating default user config");

        return new HorseGameUserConfig
        {
            DataDir = PathUtility.GetDefaultDataPath(),
            WineVersion = Compatibility.DefaultWineVersion,
            DxvkVersion = Compatibility.DefaultDxvkVersion
        };
    }
}