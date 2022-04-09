using Sadie.Database;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Friendships;
using Sadie.Shared.Networking;

namespace Sadie.Game.Players;

public interface IPlayerFactory
{
    IPlayer Create(INetworkObject networkObject, DatabaseRecord record, DatabaseReader savedSearchesReader, DatabaseReader permissionsReader, List<PlayerBadge> badges, List<PlayerFriendshipData> friendships);
    IPlayer CreateBasic(DatabaseRecord record, List<PlayerFriendshipData> friendships);
}