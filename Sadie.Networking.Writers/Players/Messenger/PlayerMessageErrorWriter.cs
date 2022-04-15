using Sadie.Game.Players.Messenger;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerMessageErrorWriter : NetworkPacketWriter
{
    public PlayerMessageErrorWriter(PlayerMessageError error, int targetId)
    {
        WriteShort(ServerPacketId.PlayerMessageError);
        WriteInteger((int) error);
        WriteInteger(targetId);
        WriteString("");
    }
}