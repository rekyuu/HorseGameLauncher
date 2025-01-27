using System;
using System.IO;
using HorseGameLauncher.Config;

namespace HorseGameLauncher.Utility;

public static class PathUtility
{
    internal static string GetDataPath()
    {
        return HorseGameUserConfig.Instance.DataDir ?? GetDefaultDataPath();
    }

    internal static string GetDefaultDataPath()
    {
        string appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string dataDir = Path.Combine(appDataDir, HorseGameLauncherConfig.ApplicationName);

        Directory.CreateDirectory(dataDir);

        return dataDir;
    }

    internal static string GetCachePath()
    {
        string dataDir = GetDefaultDataPath();
        string cacheDir = Path.Combine(dataDir, "cache");

        Directory.CreateDirectory(cacheDir);

        return cacheDir;
    }

    internal static string GetConfigFilePath()
    {
        string dataDir = GetDefaultDataPath();
        return Path.Combine(dataDir, "config.json");
    }

    internal static string GetLogsPath()
    {
        string dataDir = GetDataPath();
        string logsPath = Path.Combine(dataDir, "logs");

        Directory.CreateDirectory(logsPath);

        return logsPath;
    }

    internal static string GetComponentsPath()
    {
        string dataDir = GetDataPath();
        string componentsDir = Path.Combine(dataDir, "components");

        Directory.CreateDirectory(componentsDir);
        Directory.CreateDirectory($"{componentsDir}/wine");
        Directory.CreateDirectory($"{componentsDir}/dxvk");

        return componentsDir;
    }

    internal static string GetWinePath()
    {
        string wineVersion = HorseGameUserConfig.Instance.WineVersion ?? Compatibility.DefaultWineVersion;

        string componentsDir = GetComponentsPath();
        return Path.Combine(componentsDir, "wine", wineVersion);
    }

    internal static string GetDxvkPath()
    {
        string wineVersion = HorseGameUserConfig.Instance.DxvkVersion ?? Compatibility.DefaultDxvkVersion;

        string componentsDir = GetComponentsPath();
        return Path.Combine(componentsDir, "dxvk", wineVersion);
    }

    internal static string GetWinePrefixPath()
    {
        string dataDir = GetDataPath();
        return Path.Combine(dataDir, "prefix");
    }
}