namespace Sadie.Networking.Packets.Server.Players;

public class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    public PlayerFriendRequestsWriter() : base(ServerPacketId.PlayerFriendRequests)
    {
        WriteInt(0);
        WriteInt(0);
    }
}