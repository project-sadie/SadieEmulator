using Sadie.Game.Rooms.Users;
using Sadie.Shared.Game;

namespace Sadie.Game.Rooms.Chat;

public class RoomChatMessage(RoomUser sender, string message, IRoom room, ChatBubble bubble, int emotionId)
{
    public RoomUser Sender { get; } = sender;
    public string Message { get; } = message;
    public IRoom Room { get; } = room;
    public ChatBubble Bubble { get; } = bubble;
    public int EmotionId { get; } = emotionId;
    public DateTime CreatedAt { get; } = DateTime.Now;
}