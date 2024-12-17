using System.Diagnostics;
using Sadie.API.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Packets.Writers.Users;

namespace SadieEmulator.Tasks.Game.Rooms;

public class SendRoomUpdatesTask(IRoomRepository roomRepository) : AbstractTask
{
    public override async Task ExecuteAsync()
    {
        await Parallel.ForEachAsync(
            roomRepository.GetAllRooms(), 
            SendUpdatesForRoomAsync);
    }

    private static async ValueTask SendUpdatesForRoomAsync(IRoomLogic room, CancellationToken arg2)
    {
        var userRepo = room.UserRepository;
        var users = userRepo.GetAll();
        
        if (users.Count == 0)
        {
            return;
        }
        
        var bots = users
            .First()
            .Room
            .BotRepository
            .GetAll();

        if (bots.Count != 0)
        {
            await userRepo.BroadcastDataAsync(new RoomBotStatusWriter
            {
                Bots = bots
            });

            await userRepo.BroadcastDataAsync(new RoomBotDataWriter
            {
                Bots = bots
            });
        }
            
        var statusWriter = new RoomUserStatusWriter
        {
            Users = users
                .Where(x => x.NeedsStatusUpdate)
                .ToList()
        };

        var dataWriter = new RoomUserDataWriter
        {
            Users = users
        };

        await userRepo.BroadcastDataAsync(statusWriter);
        await userRepo.BroadcastDataAsync(dataWriter);
    }
}