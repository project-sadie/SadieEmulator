using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Database.Models.Rooms.Chat;

public class RoomChatMessage
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int PlayerId { get; set; }
    public string Message { get; set; }
    public ChatBubble ChatBubbleId { get; set; }
    public int EmotionId { get; set; }
    public RoomChatMessageType Type { get; set; }
    public DateTime CreatedAt { get; set; }
}