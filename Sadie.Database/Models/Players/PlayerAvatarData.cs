using Sadie.Enums.Unsorted;

namespace Sadie.Database.Models.Players;

public class PlayerAvatarData
{
    public int Id { get; init; }
    public long PlayerId { get; init; }
    public Player? Player { get; init; }
    public required string FigureCode { get; set; }
    public string? Motto { get; set; }
    public AvatarGender Gender { get; set; }
    public ChatBubble ChatBubbleId { get; set; }
}