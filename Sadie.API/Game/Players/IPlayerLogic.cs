using DotNetty.Transport.Channels;
using Sadie.Database.Models;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.API.Game.Players;

public interface IPlayerLogic
{
    IPlayerState State { get; }
    PlayerData? Data { get; }
    PlayerAvatarData? AvatarData { get; }
    ICollection<PlayerFurnitureItem> FurnitureItems { get; }
    ICollection<Room> Rooms { get; set; }
    IChannel? Channel { get; set; }
    INetworkObject? NetworkObject { get; set; }
    bool Authenticated { get; set; }
    int Id { get; init; }
    string Username { get; init; }
    string Email { get; init; }
    ICollection<Role> Roles { get; init; }
    List<PlayerTag> Tags { get; init; }
    DateTime CreatedAt { get; init; }
    ValueTask DisposeAsync();
    bool DeservesReward(string? rewardType, int intervalInSeconds);
    Task SendAlertAsync(string message);
    int GetAcceptedFriendshipCount();
    bool IsFriendsWith(int targetId);
    void DeleteFriendshipFor(int targetId);
    bool HasPermission(string name);
    List<PlayerFriendship> GetMergedFriendships();
}