namespace Sadie.Networking.Packets.Server.Players;

public class PlayerIgnoredUsers : NetworkPacketWriter
{
    public PlayerIgnoredUsers() : base(ServerPacketId.PlayerIgnoredUsers)
    {
        WriteInt(0);
    }
}