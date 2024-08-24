namespace Sadie.API.Game.Players.Friendships;

public interface IPlayerFriendshipRequestData
{
    long Id { get; init; }
    string Username { get; init; }
    string FigureCode { get; init; }
}