using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Inventory;
using Sadie.Game.Players.Navigator;
using Sadie.Game.Players.Subscriptions;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public interface IPlayerData : IAvatarData, IAsyncDisposable
{
    int Id { get; }
    string Username { get; }
    DateTime CreatedAt { get; }
    int HomeRoom { get; }
    IPlayerBalance Balance { get; }
    DateTime LastOnline { get; set; }
    int RespectsReceived { get; set; }
    int RespectPoints { get; set; }
    int RespectPointsPet { get; }
    PlayerNavigatorSettings NavigatorSettings { get; set; }
    PlayerSettings Settings { get; }
    List<PlayerSavedSearch> SavedSearches { get; }
    List<string> Permissions { get; }
    List<PlayerBadge> Badges { get; }
    PlayerFriendshipComponent FriendshipComponent { get; }
    bool AllowFriendRequests { get; }
    List<IPlayerSubscription> Subscriptions { get; }
    IPlayerInventoryRepository Inventory { get; }
    List<long> LikedRoomIds { get; }
}