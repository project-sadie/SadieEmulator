using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SadieEmulator.Tasks;

public class ServerTaskWorker(ILogger<ServerTaskWorker> logger, IEnumerable<IServerTask> tasks)
    : IServerTaskWorker
{
    public Task WorkAsync(CancellationToken token)
    {
        foreach (var task in tasks)
        {
            _ = RunTaskLoopAsync(task, token);
        }

        return Task.CompletedTask;
    }

    private async Task RunTaskLoopAsync(IServerTask task, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (task.WaitingToExecute())
                {
                    var sw = Stopwatch.StartNew();
                    await task.ExecuteAsync();
                    sw.Stop();

                    if (sw.Elapsed >= task.PeriodicInterval)
                    {
                        logger.LogWarning(
                            $"Task '{task.GetType().Name}' took {sw.ElapsedMilliseconds}ms to execute.");
                    }

                    task.LastExecutedTicks = Stopwatch.GetTimestamp();
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while executing server task.");
            }

            await Task.Delay(5, token);
        }
    }

    public void Dispose()
    {
        
    }
}