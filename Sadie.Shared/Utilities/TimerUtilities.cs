namespace Sadie.Shared.Utilities;

public static class TimerUtilities
{
    public static async Task RunPeriodically(TimeSpan timeSpan, Func<Task> task, CancellationToken cancellationToken)
    {
        while (await new PeriodicTimer(timeSpan).WaitForNextTickAsync(cancellationToken))
        {
            await task();
        }
    }
}