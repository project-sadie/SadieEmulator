using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Rooms.Users.HandItems;

namespace Sadie.Networking.Events.Handlers.Rooms.Users.HandItems;

[PacketId(EventHandlerId.RoomUserGiveHandItem)]
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

        await room.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = fromUser.Player.Player.Id,
            ItemId = 0
        });

        await toUser.NetworkObject.WriteToStreamAsync(new RoomUserReceivedHandItemWriter
        {
            FromId = fromUser.Player.Player.Id,
            HandItemId = handItemId
        });
        
        await room.BroadcastDataAsync(new RoomUserHandItemWriter
        {
            UserId = toUser.Player.Player.Id,
            ItemId = handItemId
        });

        fromUser.HandItemId = handItemId;
        fromUser.HandItemSet = DateTime.Now;
        
        toUser.HandItemId = handItemId;
        toUser.HandItemSet = DateTime.Now;
    }
}