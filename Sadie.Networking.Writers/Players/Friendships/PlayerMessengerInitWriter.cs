using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerMessengerInitWriter : NetworkPacketWriter
{
    public PlayerMessengerInitWriter(int maxFriends, int unknown1, int maxFriendsHc, int unknown2)
    {
        WriteShort(ServerPacketId.PlayerMessengerInit);
        WriteInteger(maxFriends);
        WriteInteger(unknown1);
        WriteInteger(maxFriendsHc);
        WriteInteger(unknown2);
    }
}