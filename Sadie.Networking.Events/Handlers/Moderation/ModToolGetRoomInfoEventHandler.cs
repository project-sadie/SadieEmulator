using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Packets.Writers.Moderation;

namespace Sadie.Networking.Events.Handlers.Moderation;

[PacketId(EventHandlerId.ModToolsRoomInfo)]
public class ModToolGetRoomInfoEventHandler(IPlayerRepository playerRepository) : INetworkPacketEventHandler
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
            Id = room.Room.Id,
            UserCount = room.UserRepository.Count,
            OwnerInRoom = room.UserRepository.TryGetById(room.Room.OwnerId, out _),
            OwnerId = room.Room.OwnerId,
            OwnerName = (await playerRepository.GetPlayerByIdAsync(room.Room.OwnerId))?.Username ?? "Unknown User",
            Unknown1 = true,
            Name = room.Room.Name,
            Description = room.Room.Description,
            Tags = room
                .Room.Tags
                .Select(t => t.Name)
                .ToList()
        });
    }
}