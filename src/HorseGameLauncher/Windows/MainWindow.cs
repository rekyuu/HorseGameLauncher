using System;
using Gtk;
using HorseGameLauncher.Config;
using Serilog;
using Application = Gtk.Application;

namespace HorseGameLauncher.Windows;

public class MainWindow : Window
{
    public MainWindow() : base(WindowType.Toplevel)
    {
        WindowPosition = WindowPosition.Center;
        DefaultSize = new Gdk.Size(320, 180);

        HeaderBar headerBar = new();
        headerBar.ShowCloseButton = true;
        headerBar.Title = HorseGameLauncherConfig.ApplicationName;

        Titlebar = headerBar;

        Grid grid = new();
        grid.Halign = Align.Center;
        grid.Valign = Align.Center;
        grid.RowSpacing = 20;
        grid.ColumnSpacing = 20;

        Button button = new();
        button.Label = "Button";
        button.Clicked += OnButtonClicked;
        grid.Attach(button, 0, 0, 1, 1);

        Button throwButton = new();
        throwButton.Label = "Throw";
        throwButton.Clicked += OnThrowButtonClicked;
        grid.Attach(throwButton, 1, 0, 1, 1);

        Button quitButton = new();
        quitButton.Label = "Quit";
        quitButton.Clicked += OnQuitButtonClicked;
        grid.Attach(quitButton, 0, 1, 2, 1);

        Child = grid;
    }

    private void OnButtonClicked(object? sender, EventArgs e)
    {
        Log.Information("Button clicked");
    }

    private void OnThrowButtonClicked(object? sender, EventArgs e)
    {
        Log.Information("Throw button clicked");
        throw new Exception("Oops!");
    }

    private void OnQuitButtonClicked(object? sender, EventArgs e)
    {
        Application.Quit();
    }
}