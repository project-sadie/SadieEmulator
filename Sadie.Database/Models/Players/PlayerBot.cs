using Sadie.Enums.Unsorted;

namespace Sadie.Database.Models.Players;

public class PlayerBot
{
    public int Id { get; init; }
    public required int PlayerId { get; init; }
    public required int? RoomId { get; set; }
    public required string Username { get; init; }
    public required string FigureCode { get; init; }
    public required string Motto { get; init; }
    public required AvatarGender Gender { get; init; }
    public ChatBubble ChatBubbleId { get; init; }
    public required DateTime CreatedAt { get; init; }
}