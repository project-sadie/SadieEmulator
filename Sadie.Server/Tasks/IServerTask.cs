namespace SadieEmulator.Tasks;

public interface IServerTask
{
    TimeSpan PeriodicInterval { get; }
    DateTime LastExecuted { get; set; }
    
    public bool WaitingToExecute()
    {
        return LastExecuted == default || DateTime.Now - LastExecuted >= PeriodicInterval;
    }

    Task ExecuteAsync();
}