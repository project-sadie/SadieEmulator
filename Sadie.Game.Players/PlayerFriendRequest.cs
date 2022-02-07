namespace Sadie.Game.Players;

public class PlayerFriendRequest
{
    public long Id { get; }
    public string Username { get; }
    public string FigureCode { get; }
    
    public PlayerFriendRequest(long id, string username, string figureCode)
    {
        Id = id;
        Username = username;
        FigureCode = figureCode;
    }
}