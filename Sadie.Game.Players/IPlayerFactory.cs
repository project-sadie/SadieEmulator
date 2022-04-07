using Sadie.Database;
using Sadie.Game.Players.Badges;

namespace Sadie.Game.Players;

public interface IPlayerFactory
{
    IPlayer Create(DatabaseRecord record, DatabaseReader savedSearchesReader, DatabaseReader permissionsReader, List<PlayerBadge> playerBadges);
    IPlayer CreateBasic(DatabaseRecord record);
}