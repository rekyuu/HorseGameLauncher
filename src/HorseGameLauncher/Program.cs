using System;
using GLib;
using Gtk;
using HorseGameLauncher.Config;
using HorseGameLauncher.Windows;
using Serilog;
using Application = Gtk.Application;
using Log = Serilog.Log;
using Menu = GLib.Menu;
using Task = System.Threading.Tasks.Task;

namespace HorseGameLauncher;

internal static class Program
{
    private static Application? _app;
    private static Window? _win;

    private static async Task Main(string[] args)
    {
        try
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.ControlledBy(HorseGameLauncherConfig.GetLogLevel())
                .WriteTo.Console()
                .CreateLogger();

            Log.Information("Starting {ApplicationName} {ApplicationVersion} on {MachineName}",
                HorseGameLauncherConfig.ApplicationName,
                HorseGameLauncherConfig.ApplicationVersion,
                Environment.MachineName);

            HorseGameUserConfig.Load();

            Application.Init();
            ExceptionManager.UnhandledException += OnUnhandledException;

            _app = new Application(HorseGameLauncherConfig.ApplicationId, ApplicationFlags.None);
            _app.Register(Cancellable.Current);

            _win = new MainWindow();
            _app.AddWindow(_win);

            Menu menu = new();
            menu.AppendItem(new GLib.MenuItem("Help", "app.help"));
            menu.AppendItem(new GLib.MenuItem("About", "app.about"));
            menu.AppendItem(new GLib.MenuItem("Quit", "app.quit"));
            _app.AppMenu = menu;

            SimpleAction helpAction = new("help", null);
            helpAction.Activated += OnHelpActivated;
            _app.AddAction(helpAction);

            SimpleAction aboutAction = new("about", null);
            aboutAction.Activated += OnAboutActivated;
            _app.AddAction(aboutAction);

            SimpleAction quitAction = new("quit", null);
            quitAction.Activated += OnQuitActivated;
            _app.AddAction(quitAction);

            _win.ShowAll();

            Application.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "A fatal exception has occurred");
        }
        finally
        {
            Log.Information("See ya!");
            await Log.CloseAndFlushAsync();
        }
    }

    private static void OnUnhandledException(UnhandledExceptionArgs args)
    {
        Log.Error("An exception was thrown in the application: {ExceptionObject}", args.ExceptionObject);
    }

    private static void OnHelpActivated(object o, ActivatedArgs args)
    {

    }

    private static void OnAboutActivated(object o, ActivatedArgs args)
    {
        AboutDialog dialog = new()
        {
            TransientFor = _win,
            ProgramName = HorseGameLauncherConfig.ApplicationName,
            Version = HorseGameLauncherConfig.ApplicationVersion,
            Comments = "A sample application for the GtkSharp project.",
            LogoIconName = "system-run-symbolic",
            License = "This sample application is licensed under public domain.",
            Website = "https://www.github.com/rekyuu/HorseGameLauncher",
            WebsiteLabel = "GitHub"
        };

        dialog.Run();
        dialog.Hide();
    }

    private static void OnQuitActivated(object o, ActivatedArgs args)
    {
        Application.Quit();
    }
}