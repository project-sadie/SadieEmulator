using Sadie.Database;

namespace Sadie.Game.Rooms;

public class RoomDao : BaseDao, IRoomDao
{
    private readonly IRoomFactory _factory;

    public RoomDao(IDatabaseProvider databaseProvider, IRoomFactory factory) : base(databaseProvider)
    {
        _factory = factory;
    }

    public async Task<Tuple<bool, Room?>> TryGetRoomById(long roomId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   `rooms`.`id`, 
                   `rooms`.`name`, 
                   `rooms`.`layout_id`, 
                   `room_settings`.`walk_diagonal`, 
                   `room_layouts`.`name` AS `layout_name`, 
                   `room_layouts`.`heightmap`,
                   `room_layouts`.`door_x`,
                   `room_layouts`.`door_y`,
                   `room_layouts`.`door_z`,
                   `room_layouts`.`door_direction`
            FROM `rooms` 
                INNER JOIN `room_settings` ON `room_settings`.`room_id` = `rooms`.`id`
                INNER JOIN `room_layouts` ON `room_layouts`.`id` = `rooms`.`layout_id`
            WHERE `rooms`.`id` = @roomId
            LIMIT 1;", new Dictionary<string, object>
        {
            { "roomId", roomId }
        });

        var (success, record) = reader.Read();

        return success && record != null ?
            new Tuple<bool, Room?>(true, _factory.CreateFromRecord(record)) : 
            new Tuple<bool, Room?>(false, null);
    }
}