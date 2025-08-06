using Sadie.Enums.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Moderation;
using Sadie.Shared.Attributes;

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