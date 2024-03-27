using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Game.Rooms.Chat;

public class RoomChatMessage(IRoomUser sender, string message, IRoom room, ChatBubble bubble, int emotionId, RoomChatMessageType type)
{
    public IRoomUser Sender { get; } = sender;
    public string Message { get; } = message;
    public IRoom Room { get; } = room;
    public ChatBubble Bubble { get; } = bubble;
    public int EmotionId { get; } = emotionId;
    public RoomChatMessageType Type { get; } = type;
    public DateTime CreatedAt { get; } = DateTime.Now;
}