using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace SadieEmulator.Tasks;

public class ServerTaskWorker(
    ILogger<ServerTaskWorker> logger, 
    IEnumerable<IServerTask> tasks) : IServerTaskWorker
{
    private Thread? _taskWorkerThread;
    
    public async Task WorkAsync(CancellationToken token)
    {
        _taskWorkerThread = new Thread(() =>
        {
            try
            {
                RunWorkerLoopAsync(token).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        })
        {
            Name = "TaskWorkerThread"
        };

        _taskWorkerThread.Start();
    }

    private async Task RunWorkerLoopAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            foreach (var task in tasks.Where(t => t.WaitingToExecute()))
            {
                await ProcessTaskAsync(task).ConfigureAwait(false);
                task.LastExecuted = DateTime.Now;
            }

            await Task.Delay(50, token).ConfigureAwait(false);
        }
    }

    private async Task ProcessTaskAsync(IServerTask task)
    {
        try
        {
            var stopwatch = Stopwatch.StartNew();
            await task.ExecuteAsync();
            stopwatch.Stop();

            if (stopwatch.Elapsed >= task.PeriodicInterval)
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
    }
}