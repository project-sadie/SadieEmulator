using Sadie.Database.Models.Players;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Game.Players.Subscriptions;
using Sadie.Game.Players.Wardrobe;
using Sadie.Shared.Unsorted.Game;
using Sadie.Shared.Unsorted.Game.Avatar;
using PlayerBadge = Sadie.Game.Players.Badges.PlayerBadge;

namespace Sadie.Game.Players;

public interface IPlayerDataFactory
{
    PlayerData Create(
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
        PlayerGameSettings settings,
        List<PlayerSavedSearch> savedSearches,
        List<string> permissions,
        long achievementScore,
        List<string> tags,
        List<PlayerBadge> badges,
        List<PlayerFriendship> friendships,
        ChatBubble chatBubble,
        bool allowFriendRequests, 
        List<IPlayerSubscription> subscriptions, 
        List<PlayerFurnitureItem> furnitureItems,
        List<long> likedRoomIds,
        Dictionary<int, PlayerWardrobeItem> wardrobeItems,
        List<PlayerRelationship> relationships);

    PlayerFriendshipComponent CreatePlayerFriendshipComponent(int playerId, List<PlayerFriendship> friendships);
}