using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask(RoomRepository roomRepository) : IServerTask
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
            
            await room.BotRepository.RunPeriodicCheckAsync();
            await room.UserRepository.RunPeriodicCheckAsync();

            var bots = room.BotRepository.GetAll();

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