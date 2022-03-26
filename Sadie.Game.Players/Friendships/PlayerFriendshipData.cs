namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipData
{
    public long Id { get; }
    public string Username { get; }
    public string FigureCode { get; }
    public PlayerFriendshipType FriendshipType { get; }

    public PlayerFriendshipData(long id, string username, string figureCode, PlayerFriendshipType friendshipType)
    {
        Id = id;
        Username = username;
        FigureCode = figureCode;
        FriendshipType = friendshipType;
    }
}