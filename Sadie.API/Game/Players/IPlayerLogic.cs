using DotNetty.Transport.Channels;
using Sadie.Database.Models;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Server;

namespace Sadie.API.Game.Players;

public interface IPlayerLogic
{
    int Id { get; init; }
    string Username { get; init; }
    string Email { get; init; }
    ICollection<Role> Roles { get; init; }
    DateTime CreatedAt { get; init; }
    PlayerData Data { get; init; }
    PlayerAvatarData? AvatarData { get; init; }
    List<PlayerTag> Tags { get; init; }
    ICollection<PlayerRoomLike> RoomLikes { get; init; }
    ICollection<PlayerRelationship> Relationships { get; init; }
    PlayerNavigatorSettings? NavigatorSettings { get; init; }
    PlayerGameSettings? GameSettings { get; init; }
    ICollection<PlayerBadge> Badges { get; init; }
    ICollection<PlayerFurnitureItem> FurnitureItems { get; init; }
    ICollection<PlayerWardrobeItem> WardrobeItems { get; init; }
    ICollection<PlayerSubscription> Subscriptions { get; init; }
    ICollection<PlayerRespect> Respects { get; init; }
    ICollection<PlayerSavedSearch> SavedSearches { get; init; }
    ICollection<PlayerFriendship> OutgoingFriendships { get; init; }
    ICollection<PlayerFriendship> IncomingFriendships { get; init; }
    ICollection<ServerPeriodicCurrencyRewardLog> RewardLogs { get; init; }
    ICollection<Room> Rooms { get; set; }
    ICollection<Group> Groups { get; init; }
    ICollection<PlayerBot> Bots { get; init; }
    ICollection<PlayerRoomVisit> RoomVisits { get; init; }
    int GetAcceptedFriendshipCount();
    List<PlayerFriendship> GetMergedFriendships();
    bool IsFriendsWith(int targetId);
    PlayerFriendship? TryGetAcceptedFriendshipFor(int targetId);
    PlayerFriendship? TryGetFriendshipFor(int targetId);
    void DeleteFriendshipFor(int targetId);
    bool HasPermission(string name);
    IPlayerState State { get; }
    IChannel? Channel { get; set; }
    INetworkObject? NetworkObject { get; set; }
    bool Authenticated { get; set; }
    ValueTask DisposeAsync();
    bool DeservesReward(string? rewardType, int intervalInSeconds);
    Task SendAlertAsync(string message);
}