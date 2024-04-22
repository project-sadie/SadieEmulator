using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Database.Models.Rooms.Chat;

public class RoomChatMessage
{
    public int Id { get; init; }
    public int RoomId { get; init; }
    public int PlayerId { get; init; }
    public string? Message { get; init; }
    public ChatBubble ChatBubbleId { get; init; }
    public int EmotionId { get; init; }
    public RoomChatMessageType TypeId { get; init; }
    public DateTime CreatedAt { get; init; }
}