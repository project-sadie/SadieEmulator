using Sadie.Database;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Friendships;

namespace Sadie.Game.Players;

public interface IPlayerFactory
{
    IPlayer Create(DatabaseRecord record, DatabaseReader savedSearchesReader, DatabaseReader permissionsReader, List<PlayerBadge> badges, List<PlayerFriendshipData> friendships);
    IPlayer CreateBasic(DatabaseRecord record);
}