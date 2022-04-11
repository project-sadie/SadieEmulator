using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerRemoveFriendsWriter : NetworkPacketWriter
{
    public PlayerRemoveFriendsWriter(List<int> playerIds)
    {
        WriteShort(ServerPacketId.PlayerRemoveFriends);
        WriteInt(0);
        WriteInt(playerIds.Count);

        foreach (var playerId in playerIds)
        {
            WriteInt(-1);
            WriteInt(playerId);
        }
    }
}