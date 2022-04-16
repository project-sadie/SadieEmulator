using Sadie.Database;
using Sadie.Game.Rooms.Chat;
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
                   `rooms`.`is_muted`, 
                   `rooms`.`max_users_allowed`,
                   
                   (SELECT `username` FROM `players` WHERE `id` = `rooms`.`owner_id`) AS `owner_name`,
                   
                   (SELECT GROUP_CONCAT(`name`) AS `comma_seperated_tags`
                    FROM `room_tags`
                    WHERE `room_id` = `rooms`.`id`
                    GROUP BY `room_id`) AS `comma_seperated_tags`,
                   
                   `room_settings`.`walk_diagonal`, 
                   `room_settings`.`access_type`, 
                   `room_settings`.`password`, 
                   
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
            (RoomAccessType) record.Get<int>("access_type"),
            record.Get<string>("password"));
        
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

    public async Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description)
    {
        return await QueryScalarAsync(@"INSERT INTO `rooms` (`name`, `layout_id`, `owner_id`, `max_users_allowed`, `description`) 
            VALUES (@name, @layoutId, @ownerId, @maxUsers, @description);
            SELECT LAST_INSERT_ID();", new Dictionary<string, object>
        {
            {"name", name},
            {"layoutId", layoutId},
            {"ownerId", ownerId},
            {"maxUsers", maxUsers},
            {"description", description}
        });
    }

    public async Task<int> CreateRoomSettings(int roomId)
    {
        return await QueryAsync(@"INSERT INTO `room_settings` (`room_id`) VALUES (@roomId);", new Dictionary<string, object>
        {
            {"roomId", roomId},
        });
    }

    public async Task<int> GetLayoutIdFromNameAsync(string name)
    {
        return await QueryScalarAsync("SELECT `id` FROM `room_layouts` WHERE `name` = @name", new Dictionary<string, object>
        {
            {"name", name}
        });
    }

    public async Task<int> CreateChatMessages(List<RoomChatMessage> messages)
    {
        var parameters = new Dictionary<string, object>();
        var query = "INSERT INTO `room_chat_messages` (`room_id`, `player_id`, `message`, `chat_bubble_id`, `created_at`) VALUES ";

        for (var i = 0; i < messages.Count; i++)
        {
            query += $"(@roomId{i}, @playerId{i}, @message{i}, @bubbleId{i}, @createdAt{i})";
            query += i + 1 >= messages.Count ? ";" : ",";
            
            parameters.Add($"roomId{i}", messages[i].Room.Id);
            parameters.Add($"playerId{i}", messages[i].Sender.Id);
            parameters.Add($"message{i}", messages[i].Message);
            parameters.Add($"bubbleId{i}", (int) messages[i].Bubble);
            parameters.Add($"createdAt{i}", messages[i].CreatedAt);
        }
        
        return await QueryAsync(query, parameters);
    }
}