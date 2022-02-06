namespace Sadie.Networking.Packets.Server.Players;

internal class PlayerFriendRequestsWriter : NetworkPacketWriter
{
    internal PlayerFriendRequestsWriter() : base(ServerPacketId.PlayerFriendRequests)
    {
        WriteInt(0);
        WriteInt(0);
    }
}