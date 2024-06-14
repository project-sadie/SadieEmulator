using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomBannedUsers)]
public class RoomBannedUsersEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required int RoomId { get; set; }
    public async Task HandleAsync(INetworkClient client)
    {
        var room = roomRepository.TryGetRoomById(RoomId);

        if (room == null)
        {
            return;
        }

        var banListMap = new Dictionary<int, string>();

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