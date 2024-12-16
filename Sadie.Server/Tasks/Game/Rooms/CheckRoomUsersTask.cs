using Sadie.API.Game.Rooms;

namespace SadieEmulator.Tasks.Game.Rooms;

public class CheckRoomUsersTask(IRoomRepository roomRepository) : AbstractTask
{
    public override async Task ExecuteAsync()
    {
        await Parallel.ForEachAsync(
            roomRepository.GetAllRooms(), 
            RunPeriodicChecksForRoomAsync);
    }

    private static async ValueTask RunPeriodicChecksForRoomAsync(IRoomLogic? room, CancellationToken ctx)
    {
        if (room == null)
        {
            return;
        }

        await room.BotRepository.RunPeriodicCheckAsync();
        await room.UserRepository.RunPeriodicCheckAsync();
    }
}