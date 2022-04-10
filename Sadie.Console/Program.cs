using Microsoft.Extensions.DependencyInjection;
using SadieEmulator;
using Serilog;

namespace Sadie.Console;

internal static class Program
{
    private static IServer? _server;
    
    private static async Task Main()
    {
        SetEventHandlers();

        var host = Startup.CreateHost();
        _server = host.Services.GetRequiredService<IServer>();
        await _server.RunAsync();
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
    }

    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Logger.Error(e.ExceptionObject.ToString());
    }
}