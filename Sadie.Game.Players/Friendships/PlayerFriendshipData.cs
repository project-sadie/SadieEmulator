using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipData
{
    public int Id { get; }
    public string Username { get; }
    public string FigureCode { get; }
    public string Motto { get; }
    public AvatarGender Gender { get; }
    
    public PlayerFriendshipData(int id,
        string username,
        string figureCode,
        string motto,
        AvatarGender gender)
    {
        Id = id;
        Username = username;
        FigureCode = figureCode;
        Motto = motto;
        Gender = gender;
    }
}