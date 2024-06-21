using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers.Users.HandItems;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.HandItems;

[PacketId(EventHandlerId.RoomUserDropHandItem)]
public class RoomUserDropHandItemEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = roomUser.Id,
            ItemId = 0
        });
    }
}