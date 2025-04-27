using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sadie.API;
using Sadie.Shared;
using Serilog;
using Serilog.Events;

namespace SadieEmulator;

internal static class Program
{
    private static IServer? _server;
    
    private static async Task Main()
    {
        SetEventHandlers();

        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, collection) => ServerServiceCollection.AddServices(collection, context.Configuration))
            .UseSerilog((hostContext, _, logger) => 
                logger.ReadFrom.Configuration(hostContext.Configuration)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning))
            .Build();
        
        WriteHeaderToConsole();
        
        _server = host.Services.GetRequiredService<IServer>();
        await _server.RunAsync();

        Console.ReadKey(true);
    }
    
    private static void WriteHeaderToConsole()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;

        Console.WriteLine(@"");
        Console.WriteLine(@"   $$$$$$\                  $$\ $$\           ");
        Console.WriteLine(@"  $$  __$$\                 $$ |\__|          ");
        Console.WriteLine(@"  $$ /  \__| $$$$$$\   $$$$$$$ |$$\  $$$$$$\  ");
        Console.WriteLine(@"  \$$$$$$\   \____$$\ $$  __$$ |$$ |$$  __$$\ ");
        Console.WriteLine(@"   \____$$\  $$$$$$$ |$$ /  $$ |$$ |$$$$$$$$ |");
        Console.WriteLine(@"  $$\   $$ |$$  __$$ |$$ |  $$ |$$ |$$   ____|");
        Console.WriteLine(@"  \$$$$$$  |\$$$$$$$ |\$$$$$$$ |$$ |\$$$$$$$\ ");
        Console.WriteLine(@"   \______/  \_______| \_______|\__| \_______|");
        Console.WriteLine(@"");

        Console.ForegroundColor = ConsoleColor.White;

        Console.WriteLine($"         You are running version {GlobalState.Version}");
        Console.WriteLine("");
    }
    
    private static void SetEventHandlers()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        AppDomain.CurrentDomain.ProcessExit += OnClose;
        
        Console.CancelKeyPress += OnClose;
    }

    private static async void OnClose(object? sender, EventArgs e)
    {
        try
        {
            if (_server == null)
            {
                return;
            }

            await _server.DisposeAsync();
        }
        catch (Exception ex)
        {
            Log.Logger.Fatal(ex.ToString());
        }
    }

    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Logger.Error(e.ExceptionObject.ToString() ?? "Unhandled exception");
    }
}