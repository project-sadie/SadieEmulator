using Sadie.Game.Players;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerStalkErrorWriter : NetworkPacketWriter
{
    public PlayerStalkErrorWriter(PlayerStalkError stalkError)
    {
        WriteShort(ServerPacketId.PlayerStalkError);
        WriteInteger((int) stalkError);
    }
}