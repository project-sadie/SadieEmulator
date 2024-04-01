using Sadie.Game.Rooms.Chat;

namespace Sadie.Game.Rooms;

public interface IRoomChatMessageDao
{
    Task<int> CreateChatMessages(List<RoomChatMessage> messages);
}