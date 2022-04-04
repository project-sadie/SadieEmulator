using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SadieEmulator;
using Serilog;

namespace Sadie.Console;

internal static class Program
{
    private static IServer? _server;
    
    private static async Task Main()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        AppDomain.CurrentDomain.ProcessExit += OnClose;
        
        System.Console.CancelKeyPress += OnClose;
        
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, collection) => ServerServiceCollection.AddServices(collection, context.Configuration))
            .UseSerilog((hostContext, _, logger) => logger.ReadFrom.Configuration(hostContext.Configuration))
            .Build();
        
        _server = host.Services.GetRequiredService<IServer>();
        
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