using System;
using System.IO;
using System.Reflection;
using Serilog.Core;
using Serilog.Events;

namespace HorseGameLauncher.Config;

internal static class HorseGameLauncherConfig
{
    internal static string ApplicationName { get; private set; } = "HorseGameLauncher";

    internal static string ApplicationVersion { get; private set; } = "0.0.0-alpha";

    internal const string ApplicationId = "me.riichi.horse-game-launcher";

    private static string LogLevel { get; set; }

    static HorseGameLauncherConfig()
    {
        Assembly? assembly = Assembly.GetEntryAssembly();

        AssemblyProductAttribute? product = assembly?.GetCustomAttribute<AssemblyProductAttribute>();
        AssemblyInformationalVersionAttribute? version = assembly?.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

        if (!string.IsNullOrEmpty(product?.Product)) ApplicationName = product.Product;
        if (!string.IsNullOrEmpty(version?.InformationalVersion)) ApplicationVersion = version.InformationalVersion;

        LogLevel = Environment.GetEnvironmentVariable("LOG_LEVEL") ?? "INFO";
    }

    internal static LoggingLevelSwitch GetLogLevel()
    {
        LoggingLevelSwitch loggingLevelSwitch = new();

        switch (LogLevel)
        {
            case "VERBOSE":
                loggingLevelSwitch.MinimumLevel = LogEventLevel.Verbose;
                break;
            case "DEBUG":
                loggingLevelSwitch.MinimumLevel = LogEventLevel.Debug;
                break;
            case "WARN":
                loggingLevelSwitch.MinimumLevel = LogEventLevel.Warning;
                break;
            case "ERROR":
                loggingLevelSwitch.MinimumLevel = LogEventLevel.Error;
                break;
            case "FATAL":
                loggingLevelSwitch.MinimumLevel = LogEventLevel.Fatal;
                break;
            // ReSharper disable once RedundantCaseLabel
            case "INFO":
            default:
                loggingLevelSwitch.MinimumLevel = LogEventLevel.Information;
                break;
        }

        return loggingLevelSwitch;
    }
}