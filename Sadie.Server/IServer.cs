namespace SadieEmulator;

public interface IServer : IDisposable
{
    Task RunAsync();
}