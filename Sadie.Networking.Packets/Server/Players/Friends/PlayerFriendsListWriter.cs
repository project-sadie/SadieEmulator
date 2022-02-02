namespace Sadie.Networking.Packets.Server.Players.Friends;

public class PlayerFriendsListWriter : NetworkPacketWriter
{
    public PlayerFriendsListWriter(int pages, int index, int total) : base(ServerPacketId.SendFriend)
    {
        WriteInt(pages);
        WriteInt(index);
        WriteInt(total);
    }
}