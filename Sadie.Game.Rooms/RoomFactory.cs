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
    
    public RoomLayout CreateModelFromRecord(DatabaseRecord record)
    {
        var doorPoint = new HPoint(record.Get<int>("door_x"),
            record.Get<int>("door_y"),
            record.Get<float>("door_z"));
        
        return new RoomLayout(
            record.Get<long>("layout_id"), 
            record.Get<string>("layout_name"), 
            record.Get<string>("heightmap"), 
            doorPoint,
            (HDirection)record.Get<int>("door_direction"));
    }
    
    public Room CreateFromRecord(DatabaseRecord record)
    {
        var model = CreateModelFromRecord(record);

        return ActivatorUtilities.CreateInstance<Room>(
            _serviceProvider, 
            record.Get<long>("id"), 
            record.Get<string>("name"), 
            model,
            record.Get<bool>("walk_diagonal"));
    }
}