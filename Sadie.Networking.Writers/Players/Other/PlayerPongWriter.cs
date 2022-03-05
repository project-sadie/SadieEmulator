using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerPongWriter : NetworkPacketWriter
{
    public PlayerPongWriter(int id)
    {
        WriteShort(ServerPacketId.PlayerPong);
        WriteInt(id);
    }
}