namespace Sadie.Networking.Packets.Server.Players;

public class PlayerIgnoredUsersWriter : NetworkPacketWriter
{
    public PlayerIgnoredUsersWriter() : base(ServerPacketId.PlayerIgnoredUsers)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
    }
}