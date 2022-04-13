using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public interface IPlayerDataFactory
{
    IPlayerData Create(
        int id,
        string username,
        DateTime createdAt,
        int homeRoom,
        string figureCode,
        string motto,
        AvatarGender gender,
        IPlayerBalance balance,
        DateTime lastOnline,
        int respectsReceived,
        int respectPoints,
        int respectPointsPet,
        PlayerNavigatorSettings navigatorSettings,
        PlayerSettings settings,
        List<PlayerSavedSearch> savedSearches,
        List<string> permissions,
        long achievementScore,
        List<string> tags,
        List<PlayerBadge> badges,
        PlayerFriendshipComponent friendshipComponent,
        int chatBubble,
        bool allowFriendRequests);
}