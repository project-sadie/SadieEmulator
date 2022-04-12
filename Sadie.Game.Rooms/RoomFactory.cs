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
    
    public RoomLayout CreateLayoutFromRecord(DatabaseRecord record)
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

    private static IRoomSettings CreateSettingsFromRecord(DatabaseRecord record)
    {
        return new RoomSettings(
            record.Get<bool>("walk_diagonal"),
            record.Get<bool>("is_muted"));
    }
    
    public Room CreateFromRecord(DatabaseRecord record)
    {
        var model = CreateLayoutFromRecord(record);
        var settings = CreateSettingsFromRecord(record);

        return ActivatorUtilities.CreateInstance<Room>(
            _serviceProvider,
            record.Get<int>("id"),
            record.Get<string>("name"),
            model,
            record.Get<int>("owner_id"),
            record.Get<string>("owner_name"),
            record.Get<string>("description"),
            record.Get<int>("score"),
            new List<string>(record.Get<string>("comma_seperated_tags").Split(",")),
            record.Get<int>("max_users_allowed"),
            settings);
    }
}