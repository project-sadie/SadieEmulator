using Sadie.Database;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Inventory;
using Sadie.Game.Players.Navigator;
using Sadie.Game.Players.Relationships;
using Sadie.Game.Players.Subscriptions;
using Sadie.Game.Players.Wardrobe;
using Sadie.Shared.Unsorted.Game;
using Sadie.Shared.Unsorted.Game.Avatar;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players;

public class PlayerDao(
    IDatabaseProvider databaseProvider,
    IPlayerFactory playerFactory,
    IPlayerDataFactory playerDataFactory,
    IPlayerBadgeDao badgeDao,
    IPlayerFriendshipDao friendshipDao,
    IPlayerSubscriptionDao subscriptionDao,
    IPlayerInventoryDao inventoryDao,
    IPlayerWardrobeDao wardrobeDao)
    : BaseDao(databaseProvider), IPlayerDao
{
    public async Task<Tuple<bool, IPlayer?>> TryGetPlayerBySsoTokenAsync(INetworkObject networkObject, string ssoToken)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                   players.id, 
                   players.username, 
                   players.role_id, 
                   players.created_at, 
                   
                   player_data.home_room_id, 
                   player_data.credit_balance, 
                   player_data.pixel_balance, 
                   player_data.seasonal_balance, 
                   player_data.gotw_points,
                   player_data.respect_points,
                   player_data.respect_points_pet,
                   player_data.last_online,
                   player_data.achievement_score,
                   player_data.allow_friend_requests,
                   
                   player_avatar_data.figure_code, 
                   player_avatar_data.motto, 
                   player_avatar_data.gender, 
                   player_avatar_data.chat_bubble_id,
                   
                   (SELECT GROUP_CONCAT(name) AS comma_separated_tags
                    FROM player_tags
                    WHERE player_id = players.id
                    GROUP BY player_id) AS comma_separated_tags,
            
                    player_navigator_settings.window_x,
                    player_navigator_settings.window_y,
                    player_navigator_settings.window_width,
                    player_navigator_settings.window_height,
                    player_navigator_settings.open_searches,

                   player_game_settings.system_volume,
                   player_game_settings.furniture_volume,
                   player_game_settings.trax_volume,
                   player_game_settings.prefer_old_chat,
                   player_game_settings.block_room_invites,
                   player_game_settings.block_camera_follow,
                   player_game_settings.ui_flags,
                   player_game_settings.show_notifications,
        
                    (SELECT COUNT(*) FROM player_respects WHERE target_player_id = players.id) AS respects_received
            FROM players 
                INNER JOIN player_data ON player_data.player_id = players.id 
                INNER JOIN player_avatar_data ON player_avatar_data.player_id = players.id 
                INNER JOIN player_navigator_settings ON player_navigator_settings.player_id = players.id 
                INNER JOIN player_game_settings ON player_game_settings.player_id = players.id 
            WHERE players.sso_token = @ssoToken LIMIT 1;", new Dictionary<string, object>
        {
            { "ssoToken", ssoToken }
        });

        var (success, record) = reader.Read();

        if (!success || record == null)
        {
            return new Tuple<bool, IPlayer?>(false, null);
        }
        
        var savedSearches = await GetSavedSearchesAsync(record.Get<int>("id"));
        
        var balance = playerFactory.CreateBalance(record.Get<long>("credit_balance"),
            record.Get<long>("pixel_balance"),
            record.Get<long>("seasonal_balance"),
            record.Get<long>("gotw_points"));

        var navigatorSettings = playerFactory.CreateNavigatorSettings(record.Get<int>("window_x"),
            record.Get<int>("window_y"),
            record.Get<int>("window_width"),
            record.Get<int>("window_height"),
            record.Get<int>("open_searches") == 1,
            0);

        var settings = playerFactory.CreateSettings(record.Get<int>("system_volume"),
            record.Get<int>("furniture_volume"),
            record.Get<int>("trax_volume"),
            record.Get<int>("prefer_old_chat") == 1,
            record.Get<int>("block_room_invites") == 1,
            record.Get<int>("block_camera_follow") == 1,
            record.Get<int>("ui_flags"),
            record.Get<int>("show_notifications") == 1);

        var playerId = record.Get<int>("id");
        var permissions = await GetPermissionsAsync(record.Get<int>("role_id"));
        var badges = await badgeDao.GetBadgesForPlayerAsync(playerId);
        var friendships = await friendshipDao.GetAllRecordsForPlayerAsync(playerId);
        var subscriptions = await subscriptionDao.GetSubscriptionsForPlayerAsync(playerId);

        var inventoryItems = await inventoryDao.GetAllAsync(playerId);
        var inventoryRepository = new PlayerInventoryRepository(inventoryItems);

        var likedRoomIds = await GetLikedRoomsAsync(playerId);
        var wardrobeItems = await wardrobeDao.GetAllRecordsForPlayerAsync(playerId);
        var relationships = await GetRelationshipsAsync(playerId);
            
        var playerData = playerDataFactory.Create(
            record.Get<int>("id"),
            record.Get<string>("username"),
            record.Get<DateTime>("created_at"),
            record.Get<int>("home_room_id"),
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female, balance,
            DateTime.TryParse(record.Get<string>("last_online"), out var timestamp) ? timestamp : DateTime.MinValue,
            record.Get<int>("respects_received"),
            record.Get<int>("respect_points"),
            record.Get<int>("respect_points_pet"),
            navigatorSettings,
            settings,
            savedSearches,
            permissions,
            record.Get<int>("achievement_score"),
            [..record.Get<string>("comma_separated_tags").Split(",")],
            badges,
            friendships,
            (ChatBubble) record.Get<int>("chat_bubble_id"),
            record.Get<int>("allow_friend_requests") == 1,
            subscriptions,
            inventoryRepository,
            likedRoomIds,
            wardrobeItems,
            relationships);
        
        var player = playerFactory.Create(
            networkObject,
            playerData);
            
        return new Tuple<bool, IPlayer?>(true, player);
    }

    private async Task<List<PlayerSavedSearch>> GetSavedSearchesAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT 
                player_id,
                search,
                filter 
            FROM player_saved_searches 
            WHERE player_id = @playerId", new Dictionary<string, object>
        {
            { "playerId", playerId }
        });
        
        var data = new List<PlayerSavedSearch>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(new PlayerSavedSearch(
                record.Get<long>("playerId"),
                record.Get<string>("search"),
                record.Get<string>("filter")));
        }
        
        return data;
    }

    private async Task<List<string>> GetPermissionsAsync(long roleId)
    {
        var reader = await GetReaderAsync(@"
            SELECT name 
            FROM player_permissions 
            WHERE id IN (SELECT permission_id FROM player_role_permissions WHERE player_role_permissions.role_id = @roleId)", 
            new Dictionary<string, object>
            {
                { "roleId", roleId }
            });
        
        var data = new List<string>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(record.Get<string>("name"));
        }

        return data;
    }
    
    private async Task<List<long>> GetLikedRoomsAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT room_id FROM player_room_likes WHERE player_id = @playerId", 
            new Dictionary<string, object>
            {
                { "playerId", playerId }
            });
        
        var data = new List<long>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(record.Get<long>("id"));
        }

        return data;
    }

    public async Task ResetSsoTokenForPlayerAsync(long id)
    {
        await QueryAsync("UPDATE players SET sso_token = '' WHERE id = @playerId", new Dictionary<string, object>
        {
            { "playerId", id }
        });
    }

    public async Task CreatePlayerRoomLikeAsync(long playerId, long roomId)
    {
        await QueryAsync("INSERT INTO player_room_likes (player_id, room_id) VALUES (@playerId, @roomId);", new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "roomId", roomId }
        });
    }
    
    public async Task<List<PlayerRelationship>> GetRelationshipsAsync(long playerId)
    {
        var reader = await GetReaderAsync(@"
            SELECT id, origin_player_id, target_player_id, type_id FROM player_relationships WHERE origin_player_id = @playerId", 
            new Dictionary<string, object>
            {
                { "playerId", playerId }
            });
        
        var data = new List<PlayerRelationship>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            data.Add(new PlayerRelationship(record.Get<int>("id"), 
                record.Get<long>("origin_player_id"), 
                record.Get<long>("target_player_id"), 
                (PlayerRelationshipType) record.Get<int>("type_id")));
        }

        return data;
    }

    public async Task UpdateRelationshipTypeAsync(int id, PlayerRelationshipType type)
    {
        await QueryAsync("UPDATE player_relationships SET type_id = @type WHERE id = @id", new Dictionary<string, object>
        {
            { "type", (int) type },
            { "id", id }
        });
    }

    public async Task<PlayerRelationship> CreateRelationshipAsync(long playerId, long targetPlayerId, PlayerRelationshipType type)
    {
        var parameters = new Dictionary<string, object>
        {
            { "playerId", playerId },
            { "targetPlayerId", targetPlayerId },
            { "type", (int) type }
        };

        var id = await QueryScalarAsync(@"
            INSERT INTO player_relationships (
                origin_player_id, target_player_id, type_id
            ) VALUES (@playerId, @targetPlayerId, @type); SELECT LAST_INSERT_ID();", parameters);

        return new PlayerRelationship(id, playerId, targetPlayerId, type);
    }

    public async Task DeleteRelationshipAsync(int id)
    {
        await QueryAsync("DELETE FROM player_relationships WHERE id = @id", new Dictionary<string, object>()
        {
            {"id", id}
        });
    }

    public async Task CleanDataAsync()
    {
        await QueryAsync("UPDATE players SET sso_token = ''; UPDATE player_data SET is_online = 0;");
    }
}