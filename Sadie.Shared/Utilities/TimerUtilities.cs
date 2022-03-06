namespace Sadie.Shared.Utilities;

public static class TimerUtilities
{
    public static async Task RunPeriodically(TimeSpan timeSpan, Func<Task> task, CancellationToken cancellationToken)
    {
        var timer = new PeriodicTimer(timeSpan);
        
        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await task();
        }
    }
}