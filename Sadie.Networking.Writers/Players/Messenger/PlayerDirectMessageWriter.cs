using Sadie.Database.Models.Players;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerDirectMessageWriter : NetworkPacketWriter
{
    public PlayerDirectMessageWriter(PlayerMessage message)
    {
        WriteShort(ServerPacketId.PlayerMessage);
        WriteInteger(message.OriginPlayerId);
        WriteString(message.Message);
        WriteLong(DateTime.Now.ToUnix() - message.CreatedAt.ToUnix());
    }
}