using EFCore.BulkExtensions;
using Sadie.Database;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class SaveRoomChatMessagesTask(RoomRepository roomRepository, SadieContext dbContext) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        var messagesToSave = new List<RoomChatMessage>();
        
        foreach (var room in roomRepository.GetAllRooms())
        {
            if (room == null)
            {
                continue;
            }

            var chatMessages = room
                .ChatMessages
                .Where(x => x.Id == 0);

            if (!chatMessages.Any())
            {
                continue;
            }

            messagesToSave.AddRange(chatMessages);
        }

        await dbContext.BulkInsertAsync(messagesToSave);
    }
}