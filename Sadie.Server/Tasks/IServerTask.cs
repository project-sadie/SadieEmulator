using System.Diagnostics;

namespace SadieEmulator.Tasks;

public interface IServerTask
{
    TimeSpan PeriodicInterval { get; }
    long LastExecutedTicks { get; set; }
    
    public bool WaitingToExecute()
    {
        if (LastExecutedTicks == 0)
        {
            return true;
        }
        
        var now = Stopwatch.GetTimestamp();
        var elapsed = now - LastExecutedTicks;
        var intervalTicks = PeriodicInterval.Ticks * Stopwatch.Frequency / TimeSpan.TicksPerSecond;
        
        return elapsed >= intervalTicks;
    }

    Task ExecuteAsync();
}