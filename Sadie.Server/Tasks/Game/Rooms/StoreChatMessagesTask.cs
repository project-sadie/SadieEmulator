using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class StoreChatMessagesTask(IRoomRepository roomRepository) : IServerTask
{
    public string Name => "StoreChatMessages";
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(30);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        foreach (var room in roomRepository.GetAllRooms())
        {
            if (room.ChatMessages.Count <= 0)
            {
                continue;
            }

            try
            {
                await roomRepository.CreateChatMessages(room.ChatMessages);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            room.ChatMessages.Clear();
        }
    }
}