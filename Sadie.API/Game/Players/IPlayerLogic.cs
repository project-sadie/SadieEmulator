using Sadie.Database.Models;
using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.API.Game.Players;

public interface IPlayerLogic
{
    IPlayerState State { get; }
    PlayerData? Data { get; }
    PlayerAvatarData? AvatarData { get; }
    ICollection<PlayerFurnitureItem> FurnitureItems { get; }
    INetworkObject? NetworkObject { get; set; }
    bool Authenticated { get; set; }
    int CurrentRoomId { get; set; }
    int Id { get; init; }
    string? Username { get; init; }
    string? Email { get; init; }
    ICollection<Role> Roles { get; init; }
    List<PlayerTag> Tags { get; init; }
    DateTime CreatedAt { get; init; }
    ValueTask DisposeAsync();
    bool DeservesReward(string? rewardType, int intervalInSeconds);
    int GetAcceptedFriendshipCount();
    bool IsFriendsWith(int targetId);
    void DeleteFriendshipFor(int targetId);
    bool HasPermission(string name);
}