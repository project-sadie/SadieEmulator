using Sadie.Enums.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
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

        await client.WriteToStreamAsync(new ModToolRoomChatLogWriter());
    }
}