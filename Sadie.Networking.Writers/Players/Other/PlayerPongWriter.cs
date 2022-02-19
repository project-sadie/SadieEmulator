namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerPongWriter : NetworkPacketWriter
{
    public PlayerPongWriter(int id) : base(ServerPacketId.PlayerPong)
    {
        WriteInt(id);
    }
}