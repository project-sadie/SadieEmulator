using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat;

public class RoomChatMessage
{
    public RoomUser Sender { get; }
    public string Message { get; }
    public IRoom Room { get; }
    public RoomChatBubble Bubble { get; }
    public int EmotionId { get; }

    public RoomChatMessage(RoomUser sender, string message, IRoom room, RoomChatBubble bubble, int emotionId)
    {
        Sender = sender;
        Message = message;
        Room = room;
        Bubble = bubble;
        EmotionId = emotionId;
    }
}