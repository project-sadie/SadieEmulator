using Sadie.Game.Players.Messenger;
using Sadie.Shared.Extensions;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerMessageWriter : NetworkPacketWriter
{
    public PlayerMessageWriter(PlayerMessage message)
    {
        WriteShort(ServerPacketId.PlayerMessage);
        WriteInteger(message.OriginId);
        WriteString(message.Message);
        WriteLong(DateTime.Now.ToUnix() - message.CreatedAt.ToUnix());
    }
}