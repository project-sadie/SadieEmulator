using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask(RoomRepository roomRepository, SadieContext dbContext) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }

    public Task ExecuteAsync()
    {
        Parallel.ForEach(roomRepository.GetAllRooms(), RunPeriodicChecksForRoom);
        return Task.CompletedTask;
    }

    private static async void RunPeriodicChecksForRoom(RoomLogic? room)
    {
        if (room == null)
        {
            return;
        }

        await room.BotRepository.RunPeriodicCheckAsync();
        await room.UserRepository.RunPeriodicCheckAsync();
    }
}