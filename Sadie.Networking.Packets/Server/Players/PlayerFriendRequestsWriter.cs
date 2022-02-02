namespace Sadie.Networking.Packets.Server.Players;

public class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    public PlayerFriendRequestsWriter() : base(ServerPacketId.PlayerFriendRequests)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
        WriteInt(0);
    }
}