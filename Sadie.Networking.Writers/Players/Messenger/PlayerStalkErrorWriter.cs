using Sadie.Game.Players;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerStalkErrorWriter : NetworkPacketWriter
{
    public PlayerStalkErrorWriter(PlayerStalkError stalkError)
    {
        WriteShort(ServerPacketId.PlayerStalkError);
        WriteInteger((int) stalkError);
    }
}