using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipData(
    int id,
    string username,
    string figureCode,
    string motto,
    AvatarGender gender)
{
    public int Id { get; } = id;
    public string Username { get; } = username;
    public string FigureCode { get; } = figureCode;
    public string Motto { get; } = motto;
    public AvatarGender Gender { get; } = gender;
}