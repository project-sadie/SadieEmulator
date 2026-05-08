using Microsoft.Extensions.Hosting;
using Sadie.Core.Shared;
using Spectre.Console;

namespace Sadie.Console.Services;

public class ConsoleBannerService : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        WriteBanner();
        WriteVersionInfo();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private static void WriteBanner()
    {
        System.Console.ForegroundColor = ConsoleColor.Magenta;

        AnsiConsole.Write(
            new Markup("[hotpink]" + @"
  /$$$$$$                  /$$ /$$          
 /$$__  $$                | $$|__/          
| $$  \__/  /$$$$$$   /$$$$$$$ /$$  /$$$$$$ 
|  $$$$$$  |____  $$ /$$__  $$| $$ /$$__  $$
 \____  $$  /$$$$$$$| $$  | $$| $$| $$$$$$$$
 /$$  \ $$ /$$__  $$| $$  | $$| $$| $$_____/
|  $$$$$$/|  $$$$$$$|  $$$$$$$| $$|  $$$$$$$
 \______/  \_______/ \_______/|__/ \_______/" + "[/]")
                .Centered()
        );

        System.Console.ForegroundColor = ConsoleColor.White;
        System.Console.WriteLine();
        System.Console.WriteLine();
    }

    private static void WriteVersionInfo()
    {
        var assembly = typeof(Server.Server).Assembly;
        var version = assembly.GetName().Version;

        if (version != null)
        {
            GlobalState.Version = version;
        }

        AnsiConsole.Write(
            new Markup($"[white]You're running version {version}[/]")
                .Centered()
        );

        System.Console.WriteLine();
        System.Console.WriteLine();
    }
}