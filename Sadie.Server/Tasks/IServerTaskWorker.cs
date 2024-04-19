namespace SadieEmulator.Tasks;

public interface IServerTaskWorker : IDisposable
{
    Task WorkAsync(CancellationToken token);
}