using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friends;

public class PlayerMessengerInitWriter : NetworkPacketWriter
{
    public PlayerMessengerInitWriter(int maxFriends, int unknown1, int maxFriendsHc, int unknown2, int unknown3, string unknown4) : base(ServerPacketId.PlayerMessengerInit)
    {
        WriteInt(maxFriends);
        WriteInt(unknown1);
        WriteInt(maxFriendsHc);
        WriteInt(unknown2);
        WriteInt(unknown3);
        WriteString(unknown4);
    }
}