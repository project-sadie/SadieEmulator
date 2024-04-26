using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerPongWriter : AbstractPacketWriter
{
    public PlayerPongWriter(int id)
    {
        WriteShort(ServerPacketId.PlayerPong);
        WriteInteger(id);
    }
}