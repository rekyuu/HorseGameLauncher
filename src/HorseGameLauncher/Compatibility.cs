using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using HorseGameLauncher.Config;
using HorseGameLauncher.Models;
using HorseGameLauncher.Utility;
using Serilog;

namespace HorseGameLauncher;

internal enum ComponentType
{
    Wine,
    Dxvk
}

internal static class Compatibility
{
    public const string DefaultWineVersion = "wine-10.0-staging-tkg-amd64";
    public const string DefaultDxvkVersion = "dxvk-2.5.3";

    private const string ComponentsRepo = "https://raw.githubusercontent.com/an-anime-team/components";
    private const string ComponentsCommit = "f09d42eca781ad9cc7ad61d8c19bf7b34eedb686";

    private static ComponentsWine[]? _wine;
    private static ComponentsDxvk[]? _dxvk;

    internal static async Task FetchComponentsIndex()
    {
        Log.Information("Updating components index");

        List<ComponentsWine> wine = [];
        wine.AddRange(await GetComponents<ComponentsWine>("wine/ge-proton") ?? []);
        wine.AddRange(await GetComponents<ComponentsWine>("wine/lutris") ?? []);
        wine.AddRange(await GetComponents<ComponentsWine>("wine/soda") ?? []);
        wine.AddRange(await GetComponents<ComponentsWine>("wine/wine-ge-proton") ?? []);
        wine.AddRange(await GetComponents<ComponentsWine>("wine/wine-staging-tkg") ?? []);
        _wine = wine.ToArray();

        List<ComponentsDxvk> dxvk = [];
        dxvk.AddRange(await GetComponents<ComponentsDxvk>("dxvk/async") ?? []);
        dxvk.AddRange(await GetComponents<ComponentsDxvk>("dxvk/gplasync") ?? []);
        dxvk.AddRange(await GetComponents<ComponentsDxvk>("dxvk/vanilla") ?? []);
        _dxvk = dxvk.ToArray();
    }

    internal static async Task DownloadComponents()
    {
        // TODO: prob should have some kind of hash or content validation

        string componentsDir = PathUtility.GetComponentsPath();
        ComponentsWine? wine = _wine?.FirstOrDefault(x =>
            x.Name == HorseGameUserConfig.Instance.WineVersion);
        ComponentsDxvk? dxvk = _dxvk?.FirstOrDefault(x =>
            x.Name == HorseGameUserConfig.Instance.DxvkVersion);

        if (wine?.Name == null) throw new Exception($"{HorseGameUserConfig.Instance.WineVersion} is not present in components");
        if (dxvk?.Name == null) throw new Exception($"{HorseGameUserConfig.Instance.DxvkVersion} is not present in components");

        string wineDir = Path.Combine(componentsDir, $"wine/{wine.Name}");
        if (!Directory.Exists(wineDir)) await DownloadComponent(wine, ComponentType.Wine);

        string dxvkDir = Path.Combine(componentsDir, $"dxvk/{dxvk.Name}");
        if (!Directory.Exists(dxvkDir)) await DownloadComponent(dxvk, ComponentType.Dxvk);
    }

    // https://github.com/goatcorp/FFXIVQuickLauncher/blob/19c603de1ec038136bdb14d65924bd525131d3fb/src/XIVLauncher.Common.Unix/Compatibility/CompatibilityTools.cs#L114
    internal static void ValidatePrefix()
    {
        Log.Information("Validating Wine prefix");
        RunInPrefix("cmd /c dir %userprofile%/Documents > nul").WaitForExit();
    }

    internal static Process RunInPrefix(string command, string workingDir = "", IDictionary<string, string>? envVars = null)
    {
        ComponentsWine? wine = _wine?.FirstOrDefault(x =>
            x.Name == HorseGameUserConfig.Instance.WineVersion);

        if (wine?.Files?.Wine64 == null) throw new Exception($"wine64 binary is missing from {HorseGameUserConfig.Instance.WineVersion}");

        string wine64 = Path.Combine(PathUtility.GetWinePath(), wine.Files.Wine64);
        ProcessStartInfo psi = new(wine64)
        {
            Arguments = command
        };

        Log.Information("Running in prefix: {FileName} {Arguments}", psi.FileName, command);

        psi.RedirectStandardInput = false;
        psi.RedirectStandardError = false;
        psi.UseShellExecute = false;
        psi.WorkingDirectory = workingDir;

        envVars ??= new Dictionary<string, string>();
        envVars.Add("WINEPREFIX", PathUtility.GetWinePrefixPath());
        // envVars.Add("WINEDLLOVERRIDES", "");
        envVars.Add("DXVK_HUD", "fps");
        // envVars.Add("DXVK_ASYNC", "0");
        // envVars.Add("WINEESYNC", "0");
        // envVars.Add("WINEFSYNC", "0");
        envVars.Add("LD_PRELOAD", Environment.GetEnvironmentVariable("LD_PRELOAD") ?? "");

        foreach (KeyValuePair<string, string> envVar in envVars)
        {
            psi.EnvironmentVariables.Add(envVar.Key, envVar.Value);
        }

        Process process = new();
        process.StartInfo = psi;
        process.ErrorDataReceived += OnProcessErrorDataReceived;
        process.Start();

        return process;
    }

    private static void OnProcessErrorDataReceived(object sender, DataReceivedEventArgs data)
    {
        Log.Error("Process error received: {Data}", data);
    }

    private static async Task<T[]?> GetComponents<T>(string path) where T : Components
    {
        string content;

        // Return the cached version if it exists
        string cachePath = GetComponentsCachePath();
        string componentPath = Path.Combine(cachePath, $"{path}.json");

        if (File.Exists(componentPath))
        {
            content = await File.ReadAllTextAsync(componentPath);
        }
        else
        {
            // Download and cache the component
            HttpRequestMessage request = new(HttpMethod.Get, $"{ComponentsRepo}/{ComponentsCommit}/{path}.json");
            HttpResponseMessage response = await HttpUtility.SendAsync(request);

            response.EnsureSuccessStatusCode();

            content = await response.Content.ReadAsStringAsync();

            await File.WriteAllTextAsync(componentPath, content);
        }

        return JsonSerializer.Deserialize<T[]>(content);
    }

    private static async Task DownloadComponent<T>(T component, ComponentType type) where T : Components
    {
        string dataDir = PathUtility.GetDataPath();
        string componentsDir = PathUtility.GetComponentsPath();

        Log.Information("Validating component: {ComponentName}", component.Name);

        if (component.Uri == null) throw new Exception($"{component.Name} does not contain a download URI");
        string filename = component.Uri.Split('/').Last();

        string downloadPath = Path.Combine(dataDir, "cache", filename);
        if (!File.Exists(downloadPath))
        {
            Log.Information("Downloading component: {ComponentName}", component.Name);
            await HttpUtility.DownloadFileAsync(component.Uri, downloadPath);
        }

        Log.Information("Extracting component: {ComponentName}", component.Name);

        string componentDir = type switch
        {
            ComponentType.Wine => "wine",
            ComponentType.Dxvk => "dxvk",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        if (downloadPath.EndsWith(".tar.gz"))
            await IoUtility.ExtractTarGz(downloadPath, Path.Combine(componentsDir, componentDir));
        else if (downloadPath.EndsWith(".tar.xz"))
            await IoUtility.ExtractTarXz(downloadPath, Path.Combine(componentsDir, componentDir));

        File.Delete(downloadPath);
    }

    private static string GetComponentsCachePath()
    {
        string cacheDir = PathUtility.GetCachePath();
        string componentsCacheDir = Path.Combine(cacheDir, $"components/{ComponentsCommit}");

        Directory.CreateDirectory(componentsCacheDir);
        Directory.CreateDirectory($"{componentsCacheDir}/wine");
        Directory.CreateDirectory($"{componentsCacheDir}/dxvk");

        return componentsCacheDir;
    }
}