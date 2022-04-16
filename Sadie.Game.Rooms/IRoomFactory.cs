using Sadie.Database;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms;

public interface IRoomFactory
{
    RoomLayout CreateLayout(int id,
        string name,
        string heightmap,
        HPoint doorPoint,
        HDirection doorDirection);
    
    IRoomSettings CreateSettings(bool walkDiagonal, RoomAccessType accessType, string password);
    
    IRoom Create(int id,
        string name,
        RoomLayout layout,
        int ownerId,
        string ownerUsername,
        string description,
        int score,
        List<string> tags,
        int maxUsersAllowed,
        IRoomSettings settings,
        List<int> playersWithRights);
}