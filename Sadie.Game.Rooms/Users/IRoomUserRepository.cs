using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUserRepository : IAsyncDisposable
{
    ICollection<IRoomUser> GetAll();
    bool TryAdd(IRoomUser user);
    bool TryGet(int id, out IRoomUser? user);
    bool TryGetById(long id, out IRoomUser? user);
    bool TryGetByUsername(string username, out IRoomUser? user);
    Task TryRemoveAsync(int id, bool hotelView = false);
    int Count { get; }
    Task BroadcastDataAsync(AbstractPacketWriter writer);
    ICollection<IRoomUser> GetAllWithRights();
    Task RunPeriodicCheckAsync();
}