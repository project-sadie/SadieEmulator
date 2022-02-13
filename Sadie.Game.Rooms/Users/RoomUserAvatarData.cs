namespace Sadie.Game.Rooms.Users;

public class RoomUserAvatarData
{
    public string Username { get; }
    public string Motto { get; }
    public string FigureCode { get; }
    public string Gender { get; }
    public long AchievementScore { get; }
    
    public RoomUserAvatarData(string username, string motto, string figureCode, string gender, long achievementScore)
    {
        Username = username;
        Motto = motto;
        FigureCode = figureCode;
        Gender = gender;
        AchievementScore = achievementScore;
    }
}