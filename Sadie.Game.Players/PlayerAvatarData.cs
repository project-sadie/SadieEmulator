namespace Sadie.Game.Players;

public class PlayerAvatarData : IPlayerAvatarData
{
    protected PlayerAvatarData(string figureCode, string motto, PlayerAvatarGender gender)
    {
        FigureCode = figureCode;
        Motto = motto;
        Gender = gender;
    }

    public string FigureCode { get; }
    public string Motto { get; }
    public PlayerAvatarGender Gender { get; }
}