using AutoMapper;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms.Chat;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.Db;
using Sadie.Db.Models.Rooms.Chat;

namespace SadieEmulator.Tasks.Game.Rooms;

public class SaveRoomChatMessagesTask(IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromSeconds(10);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    {
        var messagesToSave = new List<RoomChatMessageDto>();
        
        foreach (var room in roomRepository.GetAllRooms())
        {
            var chatMessages = room
                .Room.ChatMessages
                .Where(x => x.Id == 0)
                .ToList();

            if (chatMessages.Count == 0)
            {
                continue;
            }

            messagesToSave.AddRange(chatMessages);
        }

        var entitiesToSave = mapper.Map<List<RoomChatMessage>>(messagesToSave);
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.BulkInsertAsync(entitiesToSave);
    }
}