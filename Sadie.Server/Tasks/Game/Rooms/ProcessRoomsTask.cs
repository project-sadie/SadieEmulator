using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask(RoomRepository roomRepository, SadieContext dbContext) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }

    public async Task ExecuteAsync()
    { 
        foreach (var room in roomRepository.GetAllRooms())
        {
            if (room == null)
            {
                continue;
            }
            
            room.BotRepository.RunPeriodicCheckAsync();
            room.UserRepository.RunPeriodicCheckAsync();

            var bots = room.BotRepository.GetAll();

            if (bots.Any())
            {
                await room.UserRepository.BroadcastDataAsync(new RoomBotStatusWriter
                {
                    Bots = bots
                });

                await room.UserRepository.BroadcastDataAsync(new RoomBotDataWriter
                {
                    Bots = bots
                });
            }
        }
    }
}