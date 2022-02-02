namespace Sadie.Networking.Packets.Server.Players;

public class PlayerIgnoredUsersWriter : NetworkPacketWriter
{
    public PlayerIgnoredUsersWriter() : base(ServerPacketId.PlayerIgnoredUsers)
    {
        WriteInt(0);
    }
}