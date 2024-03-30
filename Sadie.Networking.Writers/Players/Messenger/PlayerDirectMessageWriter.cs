using Sadie.Game.Players.Messenger;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerDirectMessageWriter : NetworkPacketWriter
{
    public PlayerDirectMessageWriter(PlayerMessage message)
    {
        WriteShort(ServerPacketId.PlayerMessage);
        WriteInteger(message.OriginId);
        WriteString(message.Message);
        WriteLong(DateTime.Now.ToUnix() - message.CreatedAt.ToUnix());
    }
}