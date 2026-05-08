using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Sadie.API.Interfaces.Server.Tasks;

namespace Sadie.Server.Tasks;

public class ServerTaskWorker(
    ILogger<ServerTaskWorker> logger,
    IEnumerable<IServerTask> tasks)
    : IServerTaskWorker
{
    private readonly List<IServerTask> _tasks = tasks.ToList();
    private readonly List<Task> _runningTasks = [];
    private CancellationTokenSource? _cts;

    public Task WorkAsync(CancellationToken token)
    {
        _cts = CancellationTokenSource.CreateLinkedTokenSource(token);

        foreach (var running in _tasks.Select(task => RunPeriodicTaskAsync(task, _cts.Token)))
        {
            _runningTasks.Add(running);
        }

        return Task.CompletedTask;
    }

    private async Task RunPeriodicTaskAsync(IServerTask task, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                var now = DateTime.UtcNow;
                var last = new DateTime(task.LastExecutedTicks);

                if (now - last >= task.PeriodicInterval)
                {
                    var sw = Stopwatch.StartNew();

                    await task.ExecuteAsync();
                    task.LastExecutedTicks = DateTime.UtcNow.Ticks;

                    sw.Stop();

                    if (sw.Elapsed > task.PeriodicInterval)
                    {
                        logger.LogWarning(
                            "Task '{Task}' exceeded its interval: {Duration}ms",
                            task.GetType().Name,
                            sw.ElapsedMilliseconds
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Unhandled exception in server task '{TaskName}'",
                    task.GetType().Name);
            }

            try
            {
                await Task.Delay(50, token);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        Task.WhenAll(_runningTasks).GetAwaiter().GetResult();
        _cts?.Dispose();
    }
}
