namespace Sadie.Networking.Packets.Server.Players.Other;

public class PlayerPong : NetworkPacketWriter
{
    public PlayerPong(int id) : base(ServerPacketIds.PlayerPong)
    {
        WriteInt(id);
    }
}