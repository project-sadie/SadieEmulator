using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Messenger;

public class PlayerSearchResultWriter : NetworkPacketWriter
{
    public PlayerSearchResultWriter(ICollection<PlayerFriendshipData> friends, ICollection<PlayerData> strangers)
    {
        WriteShort(ServerPacketId.PlayerSearchResult);
        WriteInteger(friends.Count);

        foreach (var friend in friends)
        {
            WriteInteger(friend.Id);
            WriteString(friend.Username);
            WriteString(friend.Motto);
            WriteBool(false);
            WriteBool(false);
            WriteString("");
            WriteInteger(1);
            WriteString(friend.FigureCode);
            WriteString("");
        }
        
        WriteInteger(strangers.Count);

        foreach (var stranger in strangers)
        {
            WriteInteger(stranger.Id);
            WriteString(stranger.Username);
            WriteString(stranger.Motto);
            WriteBool(false);
            WriteBool(false);
            WriteString("");
            WriteInteger(1);
            WriteString(stranger.FigureCode);
            WriteString("");
        }
    }
}