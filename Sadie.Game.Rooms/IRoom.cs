using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public interface IRoom : IRoomData, IAsyncDisposable
{
    Task RunPeriodicCheckAsync();
}