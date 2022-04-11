using Sadie.Game.Players.Friendships;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friends;

public class PlayerFriendshipErrorWriter : NetworkPacketWriter
{
    public PlayerFriendshipErrorWriter(int unknown1, PlayerFriendshipError error)
    {
        WriteShort(ServerPacketId.PlayerFriendshipError);
        WriteInt(unknown1);
        WriteInt((int) error);
    }
}