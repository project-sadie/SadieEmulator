namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerPong : NetworkPacketWriter
{
    public PlayerPong(int id) : base(ServerPacketId.PlayerPong)
    {
        WriteInt(id);
    }
}