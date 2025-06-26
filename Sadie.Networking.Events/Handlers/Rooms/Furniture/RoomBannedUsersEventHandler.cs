using Sadie.API.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomBannedUsers)]
public class RoomBannedUsersEventHandler(IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required int RoomId { get; init; }
    public async Task HandleAsync(INetworkClient client)
    {
        var room = roomRepository.TryGetRoomById(RoomId);

        if (room == null)
        {
            return;
        }

        var banListMap = new Dictionary<long, string>();

        foreach (var i in room.PlayerBans.Where(x => x.ExpiresAt > DateTime.Now))
        {
            banListMap[i.PlayerId] = i.Player.Username;
        }
        
        await client.WriteToStreamAsync(new RoomBannedUsersWriter
        {
            RoomId = room.Id,
            BannedUsersMap = banListMap
        });
    }
}