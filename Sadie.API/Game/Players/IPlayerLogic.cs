using DotNetty.Transport.Channels;
using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.API.Game.Players;

public interface IPlayerLogic : IPlayer
{
    IPlayerState State { get; }
    IChannel? Channel { get; init; }
    INetworkObject? NetworkObject { get; init; }
    bool Authenticated { get; init; }
    ValueTask DisposeAsync();
    bool DeservesReward(string? rewardType, int intervalInSeconds);
    Task SendAlertAsync(string message);
}