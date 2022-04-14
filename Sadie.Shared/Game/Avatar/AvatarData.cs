namespace Sadie.Shared.Game.Avatar;

public class AvatarData : IAvatarData
{
    public AvatarData(string username, string figureCode, string motto, AvatarGender gender, long achievementScore, List<string> tags, int chatBubble)
    {
        Username = username;
        FigureCode = figureCode;
        Motto = motto;
        Gender = gender;
        CurrentRoomId = 0;
        AchievementScore = achievementScore;
        Tags = tags;
        ChatBubble = chatBubble;
    }

    public string Username { get; }
    public string FigureCode { get; set; }
    public string Motto { get; set; }
    public AvatarGender Gender { get; set; }
    public int CurrentRoomId { get; set; }
    public long AchievementScore { get; set; }
    public List<string> Tags { get; set; }
    public int ChatBubble { get; }
}