namespace Sadie.Networking.Packets.Server.Players;

public class PlayerIgnoredUsers : NetworkPacketWriter
{
    public PlayerIgnoredUsers() : base(ServerPacketIds.PlayerIgnoredUsers)
    {
        WriteInt(0);
    }
}