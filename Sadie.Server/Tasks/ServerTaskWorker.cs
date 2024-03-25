using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SadieEmulator.Tasks;

public class ServerTaskWorker(
    ILogger<ServerTaskWorker> logger, 
    List<IServerTask> tasks) : IServerTaskWorker
{
    public void Start()
    {
        Task.Run(WorkAsync);
    }

    private async Task WorkAsync()
    {
        while (true)
        {
            foreach (var task in tasks.Where(task => task.WaitingToExecute()))
            {
                task.LastExecuted = DateTime.Now;
                await ProcessTaskAsync(task);
            }

            await Task.Delay(50);
        }
    }

    private async Task ProcessTaskAsync(IServerTask task)
    {
        var stopwatch = Stopwatch.StartNew();
        await task.ExecuteAsync();
        stopwatch.Stop();

        if (stopwatch.ElapsedMilliseconds >= task.PeriodicInterval.TotalMilliseconds / 2)
        {
            logger.LogWarning($"Task '{task.Name}' took {stopwatch.ElapsedMilliseconds}ms to run.");
        }
    }
}