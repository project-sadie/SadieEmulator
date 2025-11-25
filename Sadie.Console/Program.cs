using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sadie.API;
using Sadie.Core.Shared;
using SadieEmulator;
using Serilog;
using Spectre.Console;

namespace Sadie.Console;

internal static class Program
{
    private static IServer? _server;
    
    private static async Task Main()
    {
        SetEventHandlers();
        await WriteHeaderToConsoleAsync();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, collection) => ServerServiceCollection.AddServices(collection, context.Configuration))
            .UseSerilog((hostContext, _, logger) => 
                logger.ReadFrom.Configuration(hostContext.Configuration))
            .Build();
        
        _server = host.Services.GetRequiredService<IServer>();
        
        await _server.RunAsync();
        await host.RunAsync();
    }
    
    private static async Task WriteHeaderToConsoleAsync()
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
 \______/  \_______/ \_______/|__/ \_______/" + "[/]").Centered()
        );

        System.Console.ForegroundColor = ConsoleColor.White;

        var assembly = typeof(Server).Assembly;
        var version = assembly.GetName().Version;

        if (version != null)
        {
            GlobalState.Version = version;
        }
        
        AnsiConsole.Write(
            new Markup("[white]" + $"\nYou're running version {version}" + "[/]").Centered()
        );
        
        System.Console.WriteLine();
        System.Console.WriteLine();
    }
    
    private static void SetEventHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        AppDomain.CurrentDomain.ProcessExit += OnClose;
        
        System.Console.CancelKeyPress += OnClose;
    }

    private static async void OnClose(object? sender, EventArgs e)
    {
        if (_server == null)
        {
            return;
        }

        await _server.DisposeAsync();
        _server = null;
    }

    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Logger.Error(e.ExceptionObject.ToString());
    }
}