namespace Sadie.Networking.Packets.Server.Players.Friends;

internal class PlayerFriendsListWriter : NetworkPacketWriter
{
    internal PlayerFriendsListWriter(int pages, int index, int total) : base(ServerPacketId.PlayerFriendsList)
    {
        WriteInt(pages);
        WriteInt(index);
        WriteInt(total);
    }
}