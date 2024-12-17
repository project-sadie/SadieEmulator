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
            var tasksWaiting = tasks
                .Where(x => x.WaitingToExecute())
                .ToList();
            
            await Parallel.ForEachAsync(tasksWaiting, token, async (t, _) =>
            {
                t.LastExecuted = DateTime.Now;
                await ProcessTaskAsync(t);
            });
            
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

            if (stopwatch.ElapsedMilliseconds >= task.PeriodicInterval.TotalMilliseconds)
            {
                task.LagTicks++;

                if (task.LagTicks >= 3)
                {
                    logger.LogWarning($"Task '{task.GetType().Name}' has {task.LagTicks} lag ticks.");
                }
            }
            else
            {
                task.LagTicks = 0;
            }
        }
        catch (Exception e)
        {
            logger.LogError(e.ToString());
        }
    }

    public void Dispose()
    {
    }
}