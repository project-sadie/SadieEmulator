using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SadieEmulator.Tasks;

public class ServerTaskWorker(
    ILogger<ServerTaskWorker> logger, 
    IEnumerable<IServerTask> tasks) : IServerTaskWorker
{
    public async Task WorkAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var task in tasks.Where(task => task.WaitingToExecute()))
            {
                task.LastExecuted = DateTime.Now;
                await ProcessTaskAsync(task);
            }

            await Task.Delay(50, token);
        }
    }

    private async Task ProcessTaskAsync(IServerTask task)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await task.ExecuteAsync();
            stopwatch.Stop();

            if (stopwatch.ElapsedMilliseconds >= task.PeriodicInterval.TotalMilliseconds / 2)
            {
                logger.LogWarning($"Task '{task.GetType().Name}' took {stopwatch.ElapsedMilliseconds}ms to run.");
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }
}