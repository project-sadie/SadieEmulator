using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask(RoomRepository roomRepository) : IServerTask
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