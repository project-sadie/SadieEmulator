using Sadie.Game.Rooms.Packets;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class Room : RoomData, IRoom
{
    public Room(
        long id, 
        string name, 
        RoomLayout layout, 
        long ownerId, 
        string ownerName, 
        string description, 
        int score, 
        List<string> tags, 
        int maxUsers,
        IRoomUserRepository userRepository, 
        bool walkDiagonal) : base(id, name, layout, ownerId, ownerName, description, score, tags, maxUsers, userRepository, walkDiagonal)
    {
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