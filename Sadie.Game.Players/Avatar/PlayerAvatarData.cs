namespace Sadie.Game.Players.Avatar;

public class PlayerAvatarData : IPlayerAvatarData
{
    protected PlayerAvatarData(string figureCode, string motto, PlayerAvatarGender gender, long achievementScore)
    {
        FigureCode = figureCode;
        Motto = motto;
        Gender = gender;
        AchievementScore = achievementScore;
        LastRoomLoaded = 0;
    }

    public string FigureCode { get; }
    public string Motto { get; }
    public PlayerAvatarGender Gender { get; }
    public long LastRoomLoaded { get; set; }
    public long AchievementScore { get; set; }
}