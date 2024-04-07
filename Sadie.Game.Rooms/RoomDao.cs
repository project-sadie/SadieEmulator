using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database;
using Sadie.Database.LegacyAdoNet;
using Sadie.Database.Models;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Rooms.Chat;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Database.Models.Rooms.Rights;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomDao(
    IServiceProvider provider,
    SadieContext dbContext,
    IDatabaseProvider databaseProvider) 
    : BaseDao(databaseProvider)
{
    public async Task<Tuple<bool, RoomLogic?>> TryGetRoomById(long roomId)
    {
        var reader = await GetReaderAsync(@$"
            SELECT {SelectColumns}
            FROM rooms 
                INNER JOIN room_settings ON room_settings.room_id = rooms.id
                INNER JOIN room_paint_settings ON room_paint_settings.room_id = rooms.id
                INNER JOIN room_layouts ON room_layouts.id = rooms.layout_id
            WHERE rooms.id = @roomId
            LIMIT 1;", new Dictionary<string, object>
        {
            { "roomId", roomId }
        });

        var (success, record) = reader.Read();

        if (!success || record == null)
        {
            return new Tuple<bool, RoomLogic?>(false, null);
        }

        var settings = dbContext
            .Set<RoomSettings>()
            .First(x => x.RoomId == record.Get<int>("id"));
        
        var doorPoint = new HPoint(record.Get<int>("door_x"),
            record.Get<int>("door_y"),
            record.Get<float>("door_z"));

        var furnitureItems = await dbContext
            .Set<RoomFurnitureItem>()
            .Where(x => x.RoomId == record.Get<int>("id"))
            .ToListAsync();

        var tiles = RoomHelpers.BuildTileListFromHeightMap(
            record.Get<string>("heightmap"), 
            furnitureItems);

        var layout = new RoomLayout
        {
            Id = record.Get<int>("layout_id"),
            Name = record.Get<string>("layout_name"),
            HeightMap = record.Get<string>("heightmap"),
            DoorX = doorPoint.X,
            DoorY = doorPoint.Y,
            DoorZ = doorPoint.Z,
            DoorDirection = (HDirection) record.Get<int>("door_direction")
        };

        var paintSettings = new RoomPaintSettings
        {
            FloorPaint = record.Get<string>("floor_paint"),
            WallPaint = record.Get<string>("wall_paint"),
            LandscapePaint = record.Get<string>("landscape_paint"),
        };

        var layoutData = new RoomLayoutData(record.Get<string>("heightmap"), tiles);
        var playerRoomLikes = new List<PlayerRoomLike>();
        var chatMessages = new List<RoomChatMessage>();
        var owner = new Player
        {
            Id = record.Get<int>("owner_id"),
            Username = record.Get<string>("owner_name")
        };
        return new Tuple<bool, RoomLogic?>(true, new RoomLogic(record.Get<int>("id"),
                record.Get<string>("name"),
                layout, 
            layoutData,
            owner,
            record.Get<string>("description"),
            record.Get<int>("max_users_allowed"),
            record.Get<int>("is_muted") == 1,
                provider.GetRequiredService<IRoomUserRepository>(),
            furnitureItems,
            settings,
            chatMessages,
                new List<RoomPlayerRight>(),
                paintSettings,
                new List<RoomTag>(),
                playerRoomLikes));
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

    public Task<int> SaveRoomAsync(RoomLogic room)
    {
        var settings = room.Settings;
        var paintSettings = room.PaintSettings;
        
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
            WHERE room_id = @roomId LIMIT 1;

            UPDATE room_paint_settings SET 
                floor_paint = @floorPaint,
                wall_paint = @wallPaint,
                landscape_paint = @landPaint
            WHERE room_id = @roomId LIMIT 1;", new Dictionary<string, object>
        {
            {"newName", room.Name},
            {"newDescription", room.Description},
            {"newMaxUsers", room.MaxUsersAllowed},
            {"accessType", (int) settings.AccessType},
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
            {"floorPaint", paintSettings.FloorPaint},
            {"wallPaint", paintSettings.WallPaint},
            {"landPaint", paintSettings.LandscapePaint},
            {"roomId", room.Id}
        });
    }

    public async Task<List<RoomLogic>> GetByOwnerIdAsync(int ownerId, int limit, ICollection<long> excludeIds)
    {
        var excludeClause = @$"AND rooms.id NOT IN (" +
                            string.Join(",", excludeIds.Select(n => n.ToString()).ToArray()).TrimEnd(',') + @") ";
        
        var reader = await GetReaderAsync(@$"
            SELECT 
                   {SelectColumns}
            FROM rooms 
                INNER JOIN room_settings ON room_settings.room_id = rooms.id
                INNER JOIN room_paint_settings ON room_paint_settings.room_id = rooms.id
                INNER JOIN room_layouts ON room_layouts.id = rooms.layout_id
            WHERE rooms.owner_id = @ownerId " + (excludeIds.Count > 0 ? excludeClause : "") + @" 
            LIMIT " + limit + ";", new Dictionary<string, object>
        {
            { "ownerId", ownerId }
        });

        var rooms = new List<RoomLogic>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }

            var settings = dbContext
                .Set<RoomSettings>()
                .First(x => x.RoomId == record.Get<int>("id"));
        
            var doorPoint = new HPoint(record.Get<int>("door_x"),
                record.Get<int>("door_y"),
                record.Get<float>("door_z"));

            var furnitureItems = await dbContext
                .Set<RoomFurnitureItem>()
                .Where(x => x.RoomId == record.Get<int>("id"))
                .ToListAsync();

            var tiles = RoomHelpers.BuildTileListFromHeightMap(record.Get<string>("heightmap"), furnitureItems);
        

            var layout = new RoomLayout
            {
                Id = record.Get<int>("layout_id"),
                Name = record.Get<string>("layout_name"),
                HeightMap = record.Get<string>("heightmap"),
                DoorX = doorPoint.X,
                DoorY = doorPoint.Y,
                DoorZ = doorPoint.Z,
                DoorDirection = (HDirection) record.Get<int>("door_direction")
            };

            var commaSeparatedRights = record.Get<string>("comma_separated_rights");
        
            var playersWithRights = commaSeparatedRights.Contains(",") ? [..commaSeparatedRights.Split(",").Select(long.Parse)] : 
                new List<long>();

            var paintSettings = new RoomPaintSettings
            {
                FloorPaint = record.Get<string>("floor_paint"),
                WallPaint = record.Get<string>("wall_paint"),
                LandscapePaint = record.Get<string>("landscape_paint"),
            };

            var layoutData = new RoomLayoutData(record.Get<string>("heightmap"), tiles);
            var playerRoomLikes = new List<PlayerRoomLike>();
            var chatMessages = new List<RoomChatMessage>();
            var owner = new Player
            {
                Id = record.Get<int>("owner_id"),
                Username = record.Get<string>("owner_name")
            };

            var room = new RoomLogic(record.Get<int>("id"),
                record.Get<string>("name"),
                layout, 
                layoutData,
                owner,
                record.Get<string>("description"),
                record.Get<int>("max_users_allowed"),
                record.Get<int>("is_muted") == 1,
                provider.GetRequiredService<IRoomUserRepository>(),
                furnitureItems,
                settings,
                chatMessages,
                new List<RoomPlayerRight>(),
                paintSettings,
                new List<RoomTag>(),
                playerRoomLikes);
            
            rooms.Add(room);
        }

        return rooms;
    }

    public async Task<int> GetLayoutIdFromNameAsync(string name)
    {
        return await QueryScalarAsync("SELECT id FROM room_layouts WHERE name = @name", new Dictionary<string, object>
        {
            {"name", name}
        });
    }

    private const string SelectColumns = """
                                             rooms.id,
                                             rooms.name,
                                             rooms.layout_id,
                                             rooms.owner_id,
                                             rooms.description,
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
                                             
                                             room_paint_settings.room_id,
                                             room_paint_settings.floor_paint,
                                             room_paint_settings.wall_paint,
                                             room_paint_settings.landscape_paint,
                                             
                                             room_layouts.name AS layout_name,
                                             room_layouts.heightmap,
                                             room_layouts.door_x,
                                             room_layouts.door_y,
                                             room_layouts.door_z,
                                             room_layouts.door_direction
                                         """;
}