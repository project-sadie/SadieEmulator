using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Writers.Moderation;

namespace Sadie.Networking.Events.Handlers.Moderation;

[PacketId(EventHandlerId.ModToolsRoomChatLog)]
public class ModToolGetRoomChatLogEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || 
            client.RoomUser == null || 
            !client.Player.HasPermission(PlayerPermissionName.Moderator))
        {
            return;
        }

        await client.WriteToStreamAsync(new ModToolRoomChatLogWriter
        {
            Unknown1 = 1,
            Unknown2 = 2,
            Unknown3 = "roomName",
            Unknown4 = 2,
            Unknown5 = client.RoomUser.Room.Name,
            Unknown6 = "roomId",
            Unknown7 = 1,
            Unknown8 = client.RoomUser.Room.Id,
            Messages = client
                .RoomUser
                .Room
                .ChatMessages
                .Take(150)
                .ToList()
        });
    }
}