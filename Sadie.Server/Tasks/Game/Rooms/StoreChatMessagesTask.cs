using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class StoreChatMessagesTask(IRoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        foreach (var room in roomRepository.GetAllRooms())
        {
            if (room.ChatMessages.Count <= 0)
            {
                continue;
            }

            await roomRepository.CreateChatMessages(room.ChatMessages);
            room.ChatMessages.Clear();
        }
    }
}