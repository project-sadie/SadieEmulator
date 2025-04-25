using Sadie.API.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users.HandItems;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.HandItems;

[PacketId(EventHandlerId.RoomUserDropHandItem)]
public class RoomUserDropHandItemEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUser.Player.Id,
            ItemId = 0
        });
    }
}