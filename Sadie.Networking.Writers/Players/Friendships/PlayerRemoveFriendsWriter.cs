using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerRemoveFriendsWriter : AbstractPacketWriter
{
    public PlayerRemoveFriendsWriter(List<int> playerIds)
    {
        WriteShort(ServerPacketId.PlayerRemoveFriends);
        WriteInteger(0);
        WriteInteger(playerIds.Count);

        foreach (var playerId in playerIds)
        {
            WriteInteger(-1);
            WriteInteger(playerId);
        }
    }
}