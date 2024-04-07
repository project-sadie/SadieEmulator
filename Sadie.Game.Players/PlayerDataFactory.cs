using Microsoft.Extensions.DependencyInjection;
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

public class PlayerDataFactory(IServiceProvider serviceProvider) : IPlayerDataFactory
{
    public PlayerData Create(
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
        List<PlayerRelationship> relationships)
    {
        
        var friendshipComponent = CreatePlayerFriendshipComponent(id, friendships);
        var wardrobeComponent = CreatePlayerWardrobeComponent(wardrobeItems);
        
        return new PlayerData(
            id,
            username,
            createdAt,
            homeRoom,
            figureCode,
            motto,
            gender,
            balance,
            lastOnline,
            respectsReceived,
            respectPoints,
            respectPointsPet,
            navigatorSettings,
            settings,
            savedSearches,
            permissions,
            achievementScore,
            tags,
            badges,
            friendshipComponent,
            chatBubble,
            allowFriendRequests,
            subscriptions,
            inventoryRepository,
            likedRoomIds,
            wardrobeComponent,
            relationships);
    }

    public PlayerFriendshipComponent CreatePlayerFriendshipComponent(int playerId, List<PlayerFriendship> friendships)
    {
        return ActivatorUtilities.CreateInstance<PlayerFriendshipComponent>(
            serviceProvider,
            playerId,
            friendships);
    }

    private PlayerWardrobeComponent CreatePlayerWardrobeComponent(Dictionary<int, PlayerWardrobeItem> wardrobeItems)
    {
        return ActivatorUtilities.CreateInstance<PlayerWardrobeComponent>(
            serviceProvider,
            wardrobeItems);
    }
}