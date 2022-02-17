using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace SadieEmulator;

internal static class Program
{
    private static async Task Main()
    {
        AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;

        var host = Startup.CreateDefaultHostBuilder();
        var services = host.Services;

        await services.GetRequiredService<IServer>().RunAsync();
    }

    private static void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Logger.Error(e.ExceptionObject.ToString());
    }
}