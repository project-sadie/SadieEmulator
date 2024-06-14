namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipRequestData
{
    public long Id { get; set; }
    public required string Username { get; set; }
    public required string FigureCode { get; set; }
}