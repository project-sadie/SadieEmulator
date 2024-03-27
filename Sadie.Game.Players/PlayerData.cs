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

public class PlayerData(
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
    ChatBubble chatBubble,
    bool allowFriendRequests,
    List<IPlayerSubscription> subscriptions,
    IPlayerInventoryRepository inventoryRepository,
    List<long> likedRoomIds,
    PlayerWardrobeComponent wardrobeComponent,
    List<PlayerRelationship> relationships)
    : AvatarData(username, figureCode, motto, gender, achievementScore, tags, chatBubble), IPlayerData
{
    public int Id { get; } = id;
    public DateTime CreatedAt { get; } = createdAt;
    public int HomeRoom { get; } = homeRoom;
    public IPlayerBalance Balance { get; } = balance;
    public DateTime LastOnline { get; set; } = lastOnline;
    public int RespectsReceived { get; set; } = respectsReceived;
    public int RespectPoints { get; set; } = respectPoints;
    public int RespectPointsPet { get; } = respectPointsPet;
    public PlayerNavigatorSettings NavigatorSettings { get; set; } = navigatorSettings;
    public PlayerSettings Settings { get; } = settings;
    public List<PlayerSavedSearch> SavedSearches { get; } = savedSearches;
    public List<string> Permissions { get; } = permissions;
    public List<PlayerBadge> Badges { get; } = badges;
    public PlayerFriendshipComponent FriendshipComponent { get; } = friendshipComponent;
    public bool AllowFriendRequests { get; } = allowFriendRequests;
    public List<IPlayerSubscription> Subscriptions { get; } = subscriptions;
    public IPlayerInventoryRepository Inventory { get; } = inventoryRepository;
    public List<long> LikedRoomIds { get; } = likedRoomIds;
    public PlayerWardrobeComponent WardrobeComponent { get; } = wardrobeComponent;
    public List<PlayerRelationship> Relationships { get; } = relationships;

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}