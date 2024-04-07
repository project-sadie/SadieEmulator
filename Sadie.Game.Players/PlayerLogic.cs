using Microsoft.Extensions.Logging;
using Sadie.Database.Models.Players;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerLogic(
    ILogger<PlayerLogic> logger,
    INetworkObject networkObject,
    PlayerData data,
    PlayerFriendshipComponent friendshipComponent,
    PlayerBalance balance)
    : Player
{
    public INetworkObject NetworkObject { get; } = networkObject;
    public PlayerData Data { get; } = data;
    public IPlayerState State { get; } = new PlayerState();
    public PlayerFriendshipComponent FriendshipComponent = friendshipComponent;
    public IPlayerBalance Balance { get; } = balance;
    public bool Authenticated { get; set; }
    public int CurrentRoomId { get; set; }

    public ValueTask DisposeAsync()
    {
        logger.LogInformation($"Player '{Username}' has logged out");
        return ValueTask.CompletedTask;
    }
}