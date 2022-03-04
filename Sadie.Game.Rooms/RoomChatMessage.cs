using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class RoomChatMessage
{
    public RoomUser Sender { get; }
    public string Message { get; }
    public IRoom Room { get; }
    public int BubbleId { get; }
    public int EmotionId { get; }

    public RoomChatMessage(RoomUser sender, string message, IRoom room, int bubbleId, int emotionId)
    {
        Sender = sender;
        Message = message;
        Room = room;
        BubbleId = bubbleId;
        EmotionId = emotionId;
    }
}