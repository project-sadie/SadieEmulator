using Sadie.Database;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomDao : BaseDao, IRoomDao
{
    private readonly IRoomFactory _factory;

    public RoomDao(IDatabaseProvider databaseProvider, IRoomFactory factory) : base(databaseProvider)
    {
        _factory = factory;
    }

    public async Task<Tuple<bool, IRoom?>> TryGetRoomById(long roomId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   `rooms`.`id`, 
                   `rooms`.`name`, 
                   `rooms`.`layout_id`, 
                   `rooms`.`owner_id`,
                   `rooms`.`description`,
                   `rooms`.`score`,
                   `rooms`.`max_users_allowed`,
                   
                   (SELECT `username` FROM `players` WHERE `id` = `rooms`.`owner_id`) AS `owner_name`,
                   
                   (SELECT GROUP_CONCAT(`name`) AS `comma_seperated_tags`
                    FROM `room_tags`
                    WHERE `room_id` = `rooms`.`id`
                    GROUP BY `room_id`) AS `comma_seperated_tags`,
                   
                   `room_settings`.`walk_diagonal`, 
                   `room_settings`.`is_muted`, 
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

        if (!success || record == null)
        {
            return new Tuple<bool, IRoom?>(false, null);
        }
        
        var settings = _factory.CreateSettings(
            record.Get<bool>("walk_diagonal"),
            record.Get<bool>("is_muted"));
        
        var doorPoint = new HPoint(record.Get<int>("door_x"),
            record.Get<int>("door_y"),
            record.Get<float>("door_z"));
        
        var layout = _factory.CreateLayout(
            record.Get<int>("layout_id"),
            record.Get<string>("layout_name"),
            record.Get<string>("heightmap"),
            doorPoint,
            (HDirection) record.Get<int>("door_direction"));

        return new Tuple<bool, IRoom?>(true, _factory.Create(record.Get<int>("id"),
            record.Get<string>("name"),
            layout,
            record.Get<int>("owner_id"),
            record.Get<string>("owner_name"),
            record.Get<string>("description"),
            record.Get<int>("score"),
            new List<string>(record.Get<string>("comma_seperated_tags").Split(",")),
            record.Get<int>("max_users_allowed"),
            settings));
    }
}