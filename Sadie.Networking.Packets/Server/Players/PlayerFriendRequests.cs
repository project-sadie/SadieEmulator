namespace Sadie.Networking.Packets.Server.Players;

public class PlayerFriendRequests : NetworkPacketWriter
{
    public PlayerFriendRequests() : base(ServerPacketIds.PlayerFriendRequests)
    {
        WriteInt(0);
        WriteInt(0);
    }
}