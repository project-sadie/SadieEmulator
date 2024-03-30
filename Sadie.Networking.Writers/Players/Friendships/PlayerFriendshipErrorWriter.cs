using Sadie.Game.Players.Friendships;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerFriendshipErrorWriter : NetworkPacketWriter
{
    public PlayerFriendshipErrorWriter(int unknown1, PlayerFriendshipError error)
    {
        WriteShort(ServerPacketId.PlayerFriendshipError);
        WriteInteger(unknown1);
        WriteInteger((int) error);
    }
}