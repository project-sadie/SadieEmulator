namespace Sadie.Shared.Utilities;

public static class TimerUtilities
{
    public static async Task RunPeriodically(TimeSpan timeSpan, Func<Task> task)
    {
        while (await new PeriodicTimer(timeSpan).WaitForNextTickAsync())
        {
            await task();
        }
    }
}