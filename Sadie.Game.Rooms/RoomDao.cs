using Sadie.Database;
using Sadie.Game.Rooms.Chat;
using Sadie.Game.Rooms.FurnitureItems;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomDao(
    IDatabaseProvider databaseProvider, 
    IRoomFactory factory, 
    IRoomFurnitureItemDao furnitureItemDao) 
    : BaseDao(databaseProvider), IRoomDao
{
    public async Task<Tuple<bool, IRoom?>> TryGetRoomById(long roomId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   rooms.id, 
                   rooms.name, 
                   rooms.layout_id, 
                   rooms.owner_id,
                   rooms.description,
                   rooms.score,
                   rooms.is_muted, 
                   rooms.max_users_allowed,
                   
                   (SELECT username FROM players WHERE id = rooms.owner_id) AS owner_name,
                   
                   (SELECT GROUP_CONCAT(player_id) AS comma_separated_rights
                    FROM room_player_rights
                    WHERE room_id = rooms.id
                    GROUP BY room_id) AS comma_separated_rights,
                   
                   (SELECT GROUP_CONCAT(name) AS comma_separated_tags
                    FROM room_tags
                    WHERE room_id = rooms.id
                    GROUP BY room_id) AS comma_separated_tags,
                   
                   room_settings.walk_diagonal, 
                   room_settings.access_type, 
                   room_settings.password, 
                   room_settings.who_can_mute, 
                   room_settings.who_can_kick, 
                   room_settings.who_can_ban, 
                   room_settings.allow_pets, 
                   room_settings.can_pets_eat, 
                   room_settings.hide_walls, 
                   room_settings.wall_thickness, 
                   room_settings.floor_thickness, 
                   room_settings.can_users_overlap, 
                   room_settings.chat_type, 
                   room_settings.chat_weight, 
                   room_settings.chat_speed, 
                   room_settings.chat_distance, 
                   room_settings.chat_protection, 
                   room_settings.trade_option, 
                   
                   room_layouts.name AS layout_name, 
                   room_layouts.heightmap,
                   room_layouts.door_x,
                   room_layouts.door_y,
                   room_layouts.door_z,
                   room_layouts.door_direction
            FROM rooms 
                INNER JOIN room_settings ON room_settings.room_id = rooms.id
                INNER JOIN room_layouts ON room_layouts.id = rooms.layout_id
            WHERE rooms.id = @roomId
            LIMIT 1;", new Dictionary<string, object>
        {
            { "roomId", roomId }
        });

        var (success, record) = reader.Read();

        if (!success || record == null)
        {
            return new Tuple<bool, IRoom?>(false, null);
        }

        var settings = factory.CreateSettings(
            record.Get<bool>("walk_diagonal"),
            (RoomAccessType) record.Get<int>("access_type"),
            record.Get<string>("password"),
            record.Get<int>("who_can_mute"),
            record.Get<int>("who_can_kick"),
            record.Get<int>("who_can_ban"),
            record.Get<int>("allow_pets") == 1,
            record.Get<int>("can_pets_eat") == 1,
            record.Get<int>("hide_walls") == 1,
            record.Get<int>("wall_thickness"),
            record.Get<int>("floor_thickness"),
            record.Get<int>("can_users_overlap") == 1,
            record.Get<int>("chat_type"),
            record.Get<int>("chat_weight"),
            record.Get<int>("chat_speed"),
            record.Get<int>("chat_distance"),
            record.Get<int>("chat_protection"),
            record.Get<int>("trade_option"));
        
        var doorPoint = new HPoint(record.Get<int>("door_x"),
            record.Get<int>("door_y"),
            record.Get<float>("door_z"));
        
        var layout = factory.CreateLayout(
            record.Get<int>("layout_id"),
            record.Get<string>("layout_name"),
            record.Get<string>("heightmap"),
            doorPoint,
            (HDirection) record.Get<int>("door_direction"));

        var commaSeparatedRights = record.Get<string>("comma_separated_rights");
        
        var playersWithRights = commaSeparatedRights.Contains(",") ? 
            new List<int>(commaSeparatedRights.Split(",").Select(int.Parse)) : 
            new List<int>();

        var furnitureItems = await furnitureItemDao.GetItemsForRoomAsync(record.Get<int>("id"));
        var furnitureItemRepository = new RoomFurnitureItemRepository(furnitureItems);

        return new Tuple<bool, IRoom?>(true, factory.Create(record.Get<int>("id"),
            record.Get<string>("name"),
            layout,
            record.Get<int>("owner_id"),
            record.Get<string>("owner_name"),
            record.Get<string>("description"),
            record.Get<int>("score"),
            new List<string>(record.Get<string>("comma_separated_tags").Split(",")),
            record.Get<int>("max_users_allowed"),
            settings,
            playersWithRights,
            furnitureItemRepository));
    }

    public async Task<int> CreateRoomAsync(string name, int layoutId, int ownerId, int maxUsers, string description)
    {
        return await QueryScalarAsync(@"INSERT INTO rooms (name, layout_id, owner_id, max_users_allowed, description) 
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
        return await QueryAsync(@"INSERT INTO room_settings (room_id) VALUES (@roomId);", new Dictionary<string, object>
        {
            {"roomId", roomId},
        });
    }

    public async Task<int> GetLayoutIdFromNameAsync(string name)
    {
        return await QueryScalarAsync("SELECT id FROM room_layouts WHERE name = @name", new Dictionary<string, object>
        {
            {"name", name}
        });
    }

    public async Task<int> CreateChatMessages(List<RoomChatMessage> messages)
    {
        var parameters = new Dictionary<string, object>();
        var query = "INSERT INTO room_chat_messages (room_id, player_id, message, chat_bubble_id, created_at) VALUES ";

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

    public Task<int> SaveRoomAsync(IRoom room)
    {
        var settings = room.Settings;
        
        return QueryAsync(@"
            UPDATE rooms SET 
                name = @newName, 
                description = @newDescription, 
                max_users_allowed = @newMaxUsers 
            WHERE id = @roomId LIMIT 1;

            UPDATE room_settings SET 
                access_type = @accessType,
                password = @password,
                trade_option = @tradeOption,   
                allow_pets = @allowPets,
                can_pets_eat = @canPetsEat,
                can_users_overlap = @canUsersOverlap,
                hide_walls = @hideWalls,
                wall_thickness = @wallThickness,
                floor_thickness = @floorThickness,
                who_can_mute = @whoCanMute,
                who_can_kick = @whoCanKick,
                who_can_ban = @whoCanBan,
                chat_type = @chatType,
                chat_weight = @chatWeight,
                chat_speed = @chatSpeed,
                chat_distance = @chatDistance,
                chat_protection = @chatProtection
            WHERE room_id = @roomId LIMIT 1;", new Dictionary<string, object>
        {
            {"newName", room.Name},
            {"newDescription", room.Description},
            {"newMaxUsers", room.MaxUsers},
            {"accessType", settings.AccessType},
            {"password", settings.Password},
            {"tradeOption", settings.TradeOption},
            {"allowPets", settings.AllowPets ? 1 : 0},
            {"canPetsEat", settings.CanPetsEat ? 1 : 0},
            {"canUsersOverlap", settings.CanUsersOverlap ? 1 : 0},
            {"hideWalls", settings.HideWalls ? 1 : 0},
            {"wallThickness", settings.WallThickness},
            {"floorThickness", settings.FloorThickness},
            {"whoCanMute", settings.WhoCanMute},           
            {"whoCanKick", settings.WhoCanKick},           
            {"whoCanBan", settings.WhoCanBan},
            {"chatType", settings.ChatType},    
            {"chatWeight", settings.ChatWeight},    
            {"chatSpeed", settings.ChatSpeed},    
            {"chatDistance", settings.ChatDistance},    
            {"chatProtection", settings.ChatProtection},
            {"roomId", room.Id}
        });
    }

    public async Task<List<IRoom>> GetByOwnerIdAsync(int ownerId, int limit, ICollection<long> excludeIds)
    {
        var excludeClause = @"AND rooms.id NOT IN (" +
                            string.Join(",", excludeIds.Select(n => n.ToString()).ToArray()).TrimEnd(',') + @") ";
        
        var reader = await GetReaderAsync(@"
            SELECT 
                   rooms.id, 
                   rooms.name, 
                   rooms.layout_id, 
                   rooms.owner_id,
                   rooms.description,
                   rooms.score,
                   rooms.is_muted, 
                   rooms.max_users_allowed,
                   
                   (SELECT username FROM players WHERE id = rooms.owner_id) AS owner_name,
                   
                   (SELECT GROUP_CONCAT(player_id) AS comma_separated_rights
                    FROM room_player_rights
                    WHERE room_id = rooms.id
                    GROUP BY room_id) AS comma_separated_rights,
                   
                   (SELECT GROUP_CONCAT(name) AS comma_separated_tags
                    FROM room_tags
                    WHERE room_id = rooms.id
                    GROUP BY room_id) AS comma_separated_tags,
                   
                   room_settings.walk_diagonal, 
                   room_settings.access_type, 
                   room_settings.password, 
                   room_settings.who_can_mute, 
                   room_settings.who_can_kick, 
                   room_settings.who_can_ban, 
                   room_settings.allow_pets, 
                   room_settings.can_pets_eat, 
                   room_settings.hide_walls, 
                   room_settings.wall_thickness, 
                   room_settings.floor_thickness, 
                   room_settings.can_users_overlap, 
                   room_settings.chat_type, 
                   room_settings.chat_weight, 
                   room_settings.chat_speed, 
                   room_settings.chat_distance, 
                   room_settings.chat_protection, 
                   room_settings.trade_option, 
                   
                   room_layouts.name AS layout_name, 
                   room_layouts.heightmap,
                   room_layouts.door_x,
                   room_layouts.door_y,
                   room_layouts.door_z,
                   room_layouts.door_direction
            FROM rooms 
                INNER JOIN room_settings ON room_settings.room_id = rooms.id
                INNER JOIN room_layouts ON room_layouts.id = rooms.layout_id
            WHERE rooms.owner_id = @ownerId " + (excludeIds.Count > 0 ? excludeClause : "") + @" 
            LIMIT " + limit + ";", new Dictionary<string, object>
        {
            { "ownerId", ownerId }
        });

        var rooms = new List<IRoom>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            var settings = factory.CreateSettings(
                record.Get<bool>("walk_diagonal"),
                (RoomAccessType) record.Get<int>("access_type"),
                record.Get<string>("password"),
                record.Get<int>("who_can_mute"),
                record.Get<int>("who_can_kick"),
                record.Get<int>("who_can_ban"),
                record.Get<int>("allow_pets") == 1,
                record.Get<int>("can_pets_eat") == 1,
                record.Get<int>("hide_walls") == 1,
                record.Get<int>("wall_thickness"),
                record.Get<int>("floor_thickness"),
                record.Get<int>("can_users_overlap") == 1,
                record.Get<int>("chat_type"),
                record.Get<int>("chat_weight"),
                record.Get<int>("chat_speed"),
                record.Get<int>("chat_distance"),
                record.Get<int>("chat_protection"),
                record.Get<int>("trade_option"));
        
            var doorPoint = new HPoint(record.Get<int>("door_x"),
                record.Get<int>("door_y"),
                record.Get<float>("door_z"));
        
            var layout = factory.CreateLayout(
                record.Get<int>("layout_id"),
                record.Get<string>("layout_name"),
                record.Get<string>("heightmap"),
                doorPoint,
                (HDirection) record.Get<int>("door_direction"));

            var commaSeparatedRights = record.Get<string>("comma_separated_rights");
        
            var playersWithRights = commaSeparatedRights.Contains(",") ? 
                new List<int>(commaSeparatedRights.Split(",").Select(int.Parse)) : 
                new List<int>();

            var furnitureItems = await furnitureItemDao.GetItemsForRoomAsync(record.Get<int>("id"));
            var furnitureItemRepository = new RoomFurnitureItemRepository(furnitureItems);

            var room = factory.Create(record.Get<int>("id"),
                record.Get<string>("name"),
                layout,
                record.Get<int>("owner_id"),
                record.Get<string>("owner_name"),
                record.Get<string>("description"),
                record.Get<int>("score"),
                new List<string>(record.Get<string>("comma_separated_tags").Split(",")),
                record.Get<int>("max_users_allowed"),
                settings,
                playersWithRights,
                furnitureItemRepository);
            
            rooms.Add(room);
        }

        return rooms;
    }
}