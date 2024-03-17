using System.Drawing;
using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Game.Rooms;
using Sadie.Shared.Networking;

namespace Sadie.Game.Rooms.Users;

public interface IRoomUser : IRoomUserData, IAsyncDisposable
{
    int Id { get; }
    INetworkObject NetworkObject { get; }
    HPoint Point { get; }
    HDirection DirectionHead { get; }
    HDirection Direction { get; }
    void WalkToPoint(Point point, bool useDiagonal);
    void LookAtPoint(Point point);
    Task RunPeriodicCheckAsync();
    bool HasRights();
    Task OnTalkAsync(RoomChatMessage message);
}