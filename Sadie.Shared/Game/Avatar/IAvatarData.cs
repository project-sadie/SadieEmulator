namespace Sadie.Shared.Game.Avatar;

public interface IAvatarData
{
    string Username { get; }
    string FigureCode { get; set; }
    string Motto { get; set; }
    AvatarGender Gender { get; set; }
    int CurrentRoomId { get; set; }
    long AchievementScore { get; set; }
    List<string> Tags { get; set; }
    public ChatBubble ChatBubble { get; set; }
}