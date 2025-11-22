using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Rooms.Users;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

[PacketId(EventHandlerId.RoomUserSign)]
public class RoomUserSignEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int SignId { get; init; }
    
    public Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out _, out var roomUser))
        {
            return Task.CompletedTask;
        }

        roomUser.AddStatus(RoomUserStatus.Sign, SignId.ToString());
        roomUser.SignSet = DateTime.Now;
        
        return Task.CompletedTask;
    }
}