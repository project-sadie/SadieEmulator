using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Inventory;
using Sadie.Game.Players.Navigator;
using Sadie.Game.Players.Relationships;
using Sadie.Game.Players.Subscriptions;
using Sadie.Game.Players.Wardrobe;
using Sadie.Shared.Unsorted.Game;
using Sadie.Shared.Unsorted.Game.Avatar;

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
        PlayerSettings settings,
        List<PlayerSavedSearch> savedSearches,
        List<string> permissions,
        long achievementScore,
        List<string> tags,
        List<PlayerBadge> badges,
        List<PlayerFriendship> friendships,
        ChatBubble chatBubble,
        bool allowFriendRequests, 
        List<IPlayerSubscription> subscriptions, 
        PlayerInventoryRepository inventoryRepository,
        List<long> likedRoomIds,
        Dictionary<int, PlayerWardrobeItem> wardrobeItems,
        List<PlayerRelationship> relationships);

    PlayerFriendshipComponent CreatePlayerFriendshipComponent(int playerId, List<PlayerFriendship> friendships);
}