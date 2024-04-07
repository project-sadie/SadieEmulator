using Sadie.Database;
using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class StoreChatMessagesTask(SadieContext dbContext, RoomRepository roomRepository) : IServerTask
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
            
            dbContext.RoomChatMessages.AddRange(room.ChatMessages);
            await dbContext.SaveChangesAsync();
            
            room.ChatMessages.Clear();
        }
    }
}