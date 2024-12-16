namespace SadieEmulator.Tasks;

public abstract class AbstractTask : IServerTask
{
    public virtual TimeSpan PeriodicInterval { get; } = TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }
    public int LagTicks { get; set; }
    public abstract Task ExecuteAsync();

    public virtual Task OnLagging(int ticks)
    {
        return Task.CompletedTask;
    }
}