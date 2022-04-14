using Microsoft.Extensions.DependencyInjection;
using Sadie.Database;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomFactory : IRoomFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RoomFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public RoomLayout CreateLayout(int id,
        string name,
        string heightmap,
        HPoint doorPoint,
        HDirection doorDirection)
    {
        return ActivatorUtilities.CreateInstance<RoomLayout>(_serviceProvider, id, name, heightmap, doorPoint, doorDirection);
    }

    public IRoomSettings CreateSettings(bool walkDiagonal, bool muted)
    {
        return ActivatorUtilities.CreateInstance<RoomSettings>(_serviceProvider, walkDiagonal, muted);
    }

    public IRoom Create(int id,
        string name,
        RoomLayout layout,
        int ownerId,
        string ownerUsername,
        string description,
        int score,
        List<string> tags,
        int maxUsersAllowed,
        IRoomSettings settings)
    {
        return ActivatorUtilities.CreateInstance<Room>(
            _serviceProvider,
            id, 
            name,
            layout,
            ownerId,
            ownerUsername,
            description,
            score,
            tags,
            maxUsersAllowed,
            settings);
    }
}