using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms.Packets;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class Room : RoomData, IRoom
{
    private readonly ILogger<Room> _logger;

    public Room(ILogger<Room> logger, long id, string name, RoomLayout layout, IRoomUserRepository userRepository, bool walkDiagonal) : base(id, name, layout, userRepository, walkDiagonal)
    {
        _logger = logger;
    }

    public async Task RunPeriodicCheckAsync()
    {
        foreach (var roomUser in UserRepository.GetAll())
        {
            await roomUser.RunPeriodicCheckAsync();
        }

        var users = UserRepository.GetAll();
        var statusWriter = new RoomUserStatusWriter(users).GetAllBytes();
        var dataWriter = new RoomUserDataWriter(users).GetAllBytes();

        await UserRepository.BroadcastDataAsync(statusWriter);
        await UserRepository.BroadcastDataAsync(dataWriter);
    }

    public async ValueTask DisposeAsync()
    {
        await UserRepository.DisposeAsync();
    }
}