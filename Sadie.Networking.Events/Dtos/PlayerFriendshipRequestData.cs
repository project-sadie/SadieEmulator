using Sadie.API.Game.Players.Friendships;

namespace Sadie.Networking.Events.Dtos;

public class PlayerFriendshipRequestData : IPlayerFriendshipRequestData
{
    public long Id { get; init; }
    public required string Username { get; init; }
    public required string FigureCode { get; init; }
}