using Sadie.Game.Rooms.Packets;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms;

public class Room : RoomData, IRoom
{
    public Room(
        int id, 
        string name, 
        RoomLayout layout, 
        int ownerId, 
        string ownerName, 
        string description, 
        int score, 
        List<string> tags, 
        int maxUsers,
        IRoomUserRepository userRepository, 
        IRoomSettings settings) : base(id, name, layout, ownerId, ownerName, description, score, tags, maxUsers, userRepository, settings)
    {
    }

    public async Task RunPeriodicCheckAsync()
    {
        var users = UserRepository.GetAll();
        
        foreach (var roomUser in users)
        {
            await roomUser.RunPeriodicCheckAsync();
        }

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