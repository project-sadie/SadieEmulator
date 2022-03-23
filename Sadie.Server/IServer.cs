namespace SadieEmulator;

public interface IServer : IAsyncDisposable
{
    Task RunAsync();
}