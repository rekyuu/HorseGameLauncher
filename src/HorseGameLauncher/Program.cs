using System;
using CsTools.Extensions;
using GtkDotNet;

namespace HorseGameLauncher;

internal static class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Hello World!");

        Application
            .New("org.gtk.example")
            .OnActivate(app =>
            {
                app.NewWindow()
                    .Title("Hello World!")
                    .SideEffect(win =>
                    {
                        win.Child(
                            Grid.New()
                                .Attach(
                                    Button
                                        .NewWithLabel("Button 1")
                                        .OnClicked(() =>
                                        {
                                            Console.WriteLine("Button 1 clicked");
                                        }), 0, 0, 1, 1)
                                .Attach(
                                    Button
                                        .NewWithLabel("Button 2")
                                        .OnClicked(() =>
                                        {
                                            Console.WriteLine("Button 2 clicked");
                                        }), 1, 0, 1, 1)
                                .Attach(
                                    Button
                                        .NewWithLabel("Quit")
                                        .OnClicked(() =>
                                        {
                                            win.CloseWindow();
                                        }), 0, 1, 2, 1)
                        ).Show();
                    });
            }).Run(0, IntPtr.Zero);
    }
}