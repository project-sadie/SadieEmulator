using Sadie.Networking.Serialization;

namespace Sadie.API.Game.Rooms.Users;

public interface IRoomUserRepository : IAsyncDisposable
{
    ICollection<IRoomUser> GetAll();
    bool TryAdd(IRoomUser user);
    bool TryGetById(long id, out IRoomUser? user);
    bool TryGetByUsername(string username, out IRoomUser? user);
    Task TryRemoveAsync(long id, bool notifyLeft, bool hotelView = false);
    int Count { get; }
    Task BroadcastDataAsync(AbstractPacketWriter writer);
    ICollection<IRoomUser> GetAllWithRights();
    Task RunPeriodicCheckAsync();
}