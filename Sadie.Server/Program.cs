using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace SadieEmulator;

internal static class Program
{
    private static IServer? _server;
    
    private static async Task Main()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        AppDomain.CurrentDomain.ProcessExit += OnClose;

        Console.CancelKeyPress += OnClose;
        
        var host = Startup.CreateDefaultHostBuilder();
        var services = host.Services;

        _server = services.GetRequiredService<IServer>();
        
        await _server.RunAsync();
    }

    private static async void OnClose(object? sender, EventArgs e)
    {
        if (_server == null)
        {
            return;
        }

        await _server.DisposeAsync();
    }

    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Logger.Error(e.ExceptionObject.ToString());
    }
}