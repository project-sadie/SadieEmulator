using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Rooms.Chat;

namespace SadieEmulator.Tasks.Game.Rooms;

public class SaveRoomChatMessagesTask(IRoomRepository roomRepository, IDbContextFactory<SadieContext> dbContextFactory) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        var messagesToSave = new List<RoomChatMessage>();
        
        foreach (var room in roomRepository.GetAllRooms())
        {
            var chatMessages = room
                .ChatMessages
                .Where(x => x.Id == 0)
                .ToList();

            if (chatMessages.Count == 0)
            {
                continue;
            }

            messagesToSave.AddRange(chatMessages);
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.BulkInsertAsync(messagesToSave);
    }
}