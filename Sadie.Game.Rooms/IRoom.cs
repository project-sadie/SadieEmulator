namespace Sadie.Game.Rooms;

public interface IRoom : IRoomData, IAsyncDisposable
{
    Task RunPeriodicCheckAsync();
}