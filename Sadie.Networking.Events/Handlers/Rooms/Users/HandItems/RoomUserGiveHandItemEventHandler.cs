using Sadie.Game.Rooms.Packets.Writers.Users.HandItems;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.HandItems;

[PacketId(EventHandlerIds.RoomUserGiveHandItem)]
public class RoomUserGiveHandItemEventHandler : INetworkPacketEventHandler
{
    public required int UserId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser.Room;
        
        if (!room.UserRepository.TryGetById(UserId, out var toUser) || toUser == null)
        {
            return;
        }

        var fromUser = client.RoomUser;
        var handItemId = fromUser.HandItemId;

        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = fromUser.Id,
            ItemId = 0
        });

        await toUser.NetworkObject.WriteToStreamAsync(new RoomUserReceivedHandItemWriter
        {
            FromId = fromUser.Id,
            HandItemId = handItemId
        });
        
        await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = toUser.Id,
            ItemId = handItemId
        });

        fromUser.HandItemId = 0;
        toUser.HandItemId = handItemId;
    }
}