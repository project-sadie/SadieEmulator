using Sadie.Enums.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Moderation;

namespace Sadie.Networking.Events.Handlers.Moderation;

[PacketId(EventHandlerId.ModToolsRoomInfo)]
public class ModToolGetRoomInfoEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || 
            client.RoomUser == null || 
            !client.Player.HasPermission(PlayerPermissionName.Moderator))
        {
            return;
        }

        var room = client.RoomUser.Room;

        await client.WriteToStreamAsync(new ModToolRoomInfoWriter
        {
            Id = room.Id,
            UserCount = room.UserRepository.Count,
            OwnerInRoom = room.UserRepository.TryGetById(room.OwnerId, out _),
            OwnerId = room.OwnerId,
            OwnerName = room.Owner.Username,
            Unknown1 = true,
            Name = room.Name,
            Description = room.Description,
            Tags = room
                .Tags
                .Select(t => t.Name)
                .ToList()
        });
    }
}