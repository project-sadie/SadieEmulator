using Sadie.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friends;

public class PlayerFriendsListWriter : NetworkPacketWriter
{
    public PlayerFriendsListWriter(int pages, int index, int total) : base(ServerPacketId.PlayerFriendsList)
    {
        WriteInt(pages);
        WriteInt(index);
        WriteInt(total);
    }
}