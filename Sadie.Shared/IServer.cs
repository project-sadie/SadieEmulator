namespace Sadie.Shared;

public interface IServer : IAsyncDisposable
{
    Task RunAsync();
}