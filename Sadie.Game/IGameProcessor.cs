namespace Sadie.Game;

public interface IGameProcessor : IDisposable
{
    Task ProcessAsync();
    Task Boot();
}