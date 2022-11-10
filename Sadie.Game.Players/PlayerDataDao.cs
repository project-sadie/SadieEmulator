using Sadie.Database;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Room;
using Sadie.Game.Players.Subscriptions;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public class PlayerDataDao : BaseDao, IPlayerDataDao
{
    private readonly IPlayerDataFactory _factory;
    private readonly IPlayerFriendshipRepository _friendshipRepository;
    private readonly IPlayerRoomVisitDao _roomVisitDao;

    public PlayerDataDao(IDatabaseProvider databaseProvider, 
        IPlayerDataFactory factory, 
        IPlayerFriendshipRepository friendshipRepository,
        IPlayerRoomVisitDao roomVisitDao) : base(databaseProvider)
    {
        _factory = factory;
        _friendshipRepository = friendshipRepository;
        _roomVisitDao = roomVisitDao;
    }
    
    public async Task<Tuple<bool, IPlayerData?>> TryGetPlayerData(long playerId)
    {
        var reader = await GetReaderForPlayerData("id", new Dictionary<string, object>
        {
            {"value", playerId}
        });

        var (success, record) = reader.Read();

        if (success && record != null)
        {
            var playerData = await CreateFromRecordAsync(record);
            return new Tuple<bool, IPlayerData?>(true, playerData);
        }

        return new Tuple<bool, IPlayerData?>(false, null);                  
    }

    public async Task<Tuple<bool, IPlayerData?>> TryGetPlayerDataByUsername(string username)
    {
        var reader = await GetReaderForPlayerData("username", new Dictionary<string, object>
        {
            {"value", username}
        });

        var (success, record) = reader.Read();

        if (success && record != null)
        {
            var playerData = await CreateFromRecordAsync(record);
            return new Tuple<bool, IPlayerData?>(true, playerData);
        }

        return new Tuple<bool, IPlayerData?>(false, null);
    }

    public async Task<List<IPlayerData>> GetPlayerDataForSearch(string searchQuery, int[] excludedIds)
    {
        var reader = await GetReaderAsync(@$"
            SELECT 
                   players.id, 
                   players.username, 
                   players.created_at, 
            
                   player_data.home_room_id,
                   player_data.respect_points,
                   player_data.respect_points_pet,
                   player_data.last_online,
                   player_data.achievement_score,
                   player_data.allow_friend_requests,
                   
                   player_avatar_data.figure_code, 
                   player_avatar_data.motto, 
                   player_avatar_data.gender,
                   player_avatar_data.chat_bubble_id
            
            FROM players 
                INNER JOIN player_data ON player_data.player_id = players.id 
                INNER JOIN player_avatar_data ON player_avatar_data.player_id = players.id 
            WHERE {(excludedIds.Length > 0 ? $"players.id NOT IN ({string.Join(",", excludedIds)}) AND " : "")}players.username LIKE  @searchQuery LIMIT 100;", new Dictionary<string, object>
        {
            { "searchQuery", $"%{searchQuery}%" }
        });
        
        var data = new List<IPlayerData>();
        
        while (true)
        {
            var (success, record) = reader.Read();

            if (!success || record == null)
            {
                break;
            }
            
            var playerData = await CreateFromRecordAsync(record);
            
            data.Add(playerData);
        }
        
        return data;
    }

    private async Task<IPlayerData> CreateFromRecordAsync(DatabaseRecord record)
    {
        return _factory.Create(
            record.Get<int>("id"),
            record.Get<string>("username"),
            record.Get<DateTime>("created_at"),
            record.Get<int>("home_room_id"),
            record.Get<string>("figure_code"),
            record.Get<string>("motto"),
            record.Get<char>("gender") == 'M' ? AvatarGender.Male : AvatarGender.Female,
            null, // balance
            record.Get<DateTime>("last_online"),
            0, // respect received
            record.Get<int>("respect_points"),
            record.Get<int>("respect_points_pet"),
            null, // navigator settings
            null, // settings
            null, // saved searches
            new List<string>(), // permissions
            record.Get<long>("achievement_score"),
            new List<string>(), // tags
            new List<PlayerBadge>(), // badges
            await _friendshipRepository.GetAllRecordsForPlayerAsync(record.Get<int>("id")), // friendship component
            record.Get<int>("chat_bubble_id"),
            record.Get<int>("allow_friend_requests") == 1,
            new List<IPlayerSubscription>() // subscriptions
        );
    }

    private async Task<DatabaseReader> GetReaderForPlayerData(string column, Dictionary<string, object> parameters)
    {
        return await GetReaderAsync(@$"
            SELECT 
                   players.id, 
                   players.username, 
                   players.created_at, 
            
                   player_data.home_room_id,
                   player_data.respect_points,
                   player_data.respect_points_pet,
                   player_data.last_online,
                   player_data.achievement_score,
                   player_data.allow_friend_requests,
                   
                   player_avatar_data.figure_code, 
                   player_avatar_data.motto, 
                   player_avatar_data.gender,
                   player_avatar_data.chat_bubble_id
            
            FROM players 
                INNER JOIN player_data ON player_data.player_id = players.id 
                INNER JOIN player_avatar_data ON player_avatar_data.player_id = players.id 
            WHERE players.{column} = @value LIMIT 1;", parameters);
    }

    public async Task MarkPlayerAsOnlineAsync(long id)
    {
        await QueryAsync("UPDATE player_data SET is_online = 1, last_online = @lastOnline WHERE player_id = @profileId", new Dictionary<string, object>
        {
            { "lastOnline", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") },
            { "profileId", id }
        });
    }

    public async Task MarkPlayerAsOfflineAsync(IPlayerData playerData, IPlayerState playerState)
    {
        await QueryAsync(@"UPDATE player_data 
            SET 
                is_online = 0, 
                credit_balance = @creditBalance, 
                pixel_balance = @pixelBalance,
                seasonal_balance = @seasonalBalance,
                gotw_points = @gotwPoints,
                respect_points = @respectPoints,
                respect_points_pet = @respectPointsPet,
                achievement_score = @achievementScore
            WHERE player_id = @playerId", new Dictionary<string, object>
        {
            { "creditBalance", playerData.Balance.Credits },
            { "pixelBalance", playerData.Balance.Pixels },
            { "seasonalBalance", playerData.Balance.Seasonal },
            { "gotwPoints", playerData.Balance.Gotw },
            { "respectPoints", playerData.RespectPoints },
            { "respectPointsPet", playerData.RespectPointsPet },
            { "achievementScore", playerData.AchievementScore },
            { "playerId", playerData.Id }
        });

        await QueryAsync(@"UPDATE player_avatar_data 
            SET 
                figure_code = @figureCode, 
                motto = @motto, 
                gender = @gender
            WHERE player_id = @playerId", new Dictionary<string, object>
        {
            { "figureCode", playerData.FigureCode },
            { "motto", playerData.Motto },
            { "gender", playerData.Gender == AvatarGender.Male ? "M" : "F" },
            { "playerId", playerData.Id }
        });

        if (playerState.RoomVisits.Count > 0)
        {
            await _roomVisitDao.CreateAsync(playerState.RoomVisits);
        }
    }
}