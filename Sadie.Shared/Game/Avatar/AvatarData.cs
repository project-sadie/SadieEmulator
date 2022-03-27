namespace Sadie.Shared.Game.Avatar;

public class AvatarData : IAvatarData
{
    public AvatarData(string username, string figureCode, string motto, AvatarGender gender, long achievementScore, List<string> tags)
    {
        Username = username;
        FigureCode = figureCode;
        Motto = motto;
        Gender = gender;
        LastRoomLoaded = 0;
        AchievementScore = achievementScore;
        Tags = tags;
    }

    public string Username { get; }
    public string FigureCode { get; set; }
    public string Motto { get; set; }
    public AvatarGender Gender { get; set; }
    public long LastRoomLoaded { get; set; }
    public long AchievementScore { get; set; }
    public List<string> Tags { get; set; }
}