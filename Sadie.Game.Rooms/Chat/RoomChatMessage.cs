using Sadie.Game.Rooms.Users;
using Sadie.Shared.Game;

namespace Sadie.Game.Rooms.Chat;

public class RoomChatMessage
{
    public RoomUser Sender { get; }
    public string Message { get; }
    public IRoom Room { get; }
    public ChatBubble Bubble { get; }
    public int EmotionId { get; }
    public DateTime CreatedAt { get; }

    public RoomChatMessage(RoomUser sender, string message, IRoom room, ChatBubble bubble, int emotionId)
    {
        Sender = sender;
        Message = message;
        Room = room;
        Bubble = bubble;
        EmotionId = emotionId;
        CreatedAt = DateTime.Now;
    }
}