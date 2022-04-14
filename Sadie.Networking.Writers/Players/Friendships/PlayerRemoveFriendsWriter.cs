using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Friendships;

public class PlayerRemoveFriendsWriter : NetworkPacketWriter
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