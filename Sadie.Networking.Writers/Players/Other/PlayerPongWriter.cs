using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerPongWriter : NetworkPacketWriter
{
    public PlayerPongWriter(int id) : base(ServerPacketId.PlayerPong)
    {
        WriteInt(id);
    }
}