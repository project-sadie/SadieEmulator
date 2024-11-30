namespace Sadie.API;

public interface IServer : IAsyncDisposable
{
    Task RunAsync();
}