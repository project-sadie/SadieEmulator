using Sadie.Database.Models.Players;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Game.Players.Subscriptions;
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
    PlayerGameSettings settings,
    List<PlayerSavedSearch> savedSearches,
    List<string> permissions,
    long achievementScore,
    List<string> tags,
    List<PlayerBadge> badges,
    PlayerFriendshipComponent friendshipComponent,
    ChatBubble chatBubble,
    bool allowFriendRequests,
    List<IPlayerSubscription> subscriptions,
    List<PlayerFurnitureItem> furnitureItems,
    List<long> likedRoomIds,
    List<PlayerWardrobeItem> wardrobeItems,
    List<PlayerRelationship> relationships)
    : AvatarData(username, figureCode, motto, gender, achievementScore, tags, chatBubble)
{
    public int Id { get; } = id;
    public DateTime CreatedAt { get; } = createdAt;
    public int HomeRoom { get; } = homeRoom;
    public IPlayerBalance Balance { get; } = balance;
    public DateTime LastOnline { get; set; } = lastOnline;
    public int RespectsReceived { get; set; } = respectsReceived;
    public int RespectPoints { get; set; } = respectPoints;
    public int RespectPointsPet { get; } = respectPointsPet;
    public bool AllowFriendRequests { get; } = allowFriendRequests;
    public PlayerNavigatorSettings NavigatorSettings { get; set; } = navigatorSettings;
    public PlayerGameSettings Settings { get; } = settings;
    public List<PlayerSavedSearch> SavedSearches { get; } = savedSearches;
    public List<string> Permissions { get; } = permissions;
    public List<PlayerBadge> Badges { get; } = badges;
    public PlayerFriendshipComponent FriendshipComponent { get; } = friendshipComponent;
    public List<IPlayerSubscription> Subscriptions { get; } = subscriptions;
    public List<PlayerFurnitureItem> FurnitureItems { get; } = furnitureItems;
    public List<long> LikedRoomIds { get; } = likedRoomIds;
    public List<PlayerWardrobeItem> WardrobeItems { get; } = wardrobeItems;
    public List<PlayerRelationship> Relationships { get; } = relationships;

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}