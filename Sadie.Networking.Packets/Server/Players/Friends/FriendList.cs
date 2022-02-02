namespace Sadie.Networking.Packets.Server.Players.Friends;

public class FriendList : NetworkPacketWriter
{
    public FriendList(int pages, int index, int total) : base(ServerPacketId.SendFriend)
    {
        WriteInt(pages);
        WriteInt(index);
        WriteInt(total);
    }
}