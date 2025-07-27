using System.Diagnostics;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API;
using Sadie.Db;
using Sadie.Game.Players.Options;
using Sadie.Networking;
using Sadie.Networking.Client;
using SadieEmulator.Tasks;
using Serilog;

namespace SadieEmulator;

public class Server(ILogger<Server> logger,
    IServerTaskWorker taskWorker,
    INetworkListener networkListener,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IDbContextFactory<SadieMigrationsDbContext> dbContextFactoryMigrate,
    IOptions<PlayerOptions> playerOptions,
    INetworkClientRepository networkClientRepository,
    IConfiguration config) : IServer
{
    private readonly CancellationTokenSource _tokenSource = new();
    
    public async Task RunAsync()
    {
        var stopwatch = Stopwatch.StartNew();
        
        Log.Logger.Information("Booting up...");
        
        await MigrateIfNeededAsync();
        await CleanUpDataAsync();
        LoadPlugins();

        if (playerOptions.Value.CanReuseSsoTokens)
        {
            Log.Logger.Warning($"Reusable SSO tokens activated, this results in reduced security.");
        }
        
        taskWorker.WorkAsync(_tokenSource.Token);

        stopwatch.Stop();

        logger.LogInformation($"Server booted up in {Math.Round(stopwatch.Elapsed.TotalMilliseconds)}ms");
        
        await StartListeningForConnectionsAsync();
    }

    private async Task MigrateIfNeededAsync()
    {
        await using var context = await dbContextFactoryMigrate.CreateDbContextAsync();
        
        var applied = await context.Database.GetAppliedMigrationsAsync();
        var hasSetupDb = applied.Any(m => m.Contains("InitialCreate"));

        if (!hasSetupDb)
        {
            try
            {
                logger.LogWarning($"Running initial migrations");
                await context.Database.MigrateAsync();

                logger.LogWarning($"Seeding initial data");
                await SeedData.SeedInitialDataAsync(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }

    private void LoadPlugins()
    {
        var pluginFolder = config.GetValue<string>("PluginDirectory");

        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console().CreateLogger();

        if (string.IsNullOrEmpty(pluginFolder) || !Directory.Exists(pluginFolder))
        {
            return;
        }
        
        foreach (var plugin in Directory.GetFiles(pluginFolder, "*.dll", SearchOption.AllDirectories))
        {
            Assembly.LoadFile(plugin);
            Log.Logger.Warning($"Loaded plugin: {Path.GetFileNameWithoutExtension(plugin)}");
        }
    }

    private async Task StartListeningForConnectionsAsync()
    {
        networkListener.Bootstrap();
        await networkListener.ListenAsync();
    }

    private async Task CleanUpDataAsync()
    {
        await using var context = await dbContextFactory.CreateDbContextAsync();

        await context
            .PlayerData
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.IsOnline, b => false));
    }

    public async ValueTask DisposeAsync()
    {
        await _tokenSource.CancelAsync();
        
        logger.LogWarning("Server is about to shut down...");

        await networkClientRepository.DisposeAsync();
    }
}