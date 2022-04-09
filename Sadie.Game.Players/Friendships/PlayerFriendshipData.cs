namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipData
{
    public long Id { get; }
    public string Username { get; }
    public string FigureCode { get; }
    public PlayerFriendshipStatus Status { get; set; }
    public PlayerFriendshipType Type { get; }

    public PlayerFriendshipData(long id, string username, string figureCode, PlayerFriendshipStatus status, PlayerFriendshipType type)
    {
        Id = id;
        Username = username;
        FigureCode = figureCode;        
        Status = status;
        Type = type;
    }
}