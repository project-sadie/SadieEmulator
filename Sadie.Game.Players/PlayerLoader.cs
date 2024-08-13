using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Enums.Unsorted;

namespace Sadie.Game.Players;

public static class PlayerLoader
{
    private static async Task<int> GetPlayerIdFromTokenAsync(DbConnection dbConnection, string token, DateTime tokenExpires)
    {
        var tokenCommand = dbConnection.CreateCommand();
        
        tokenCommand.Parameters.Add(new MySqlParameter("token", token));
        tokenCommand.Parameters.Add(new MySqlParameter("expires", tokenExpires.ToString("s")));
        tokenCommand.CommandText = "SELECT id, player_id FROM player_sso_tokens WHERE token = @token";

        var tokenReader = await tokenCommand.ExecuteReaderAsync();

        if (!await tokenReader.ReadAsync())
        {
            return 0;
        }
        
        var tokenUpdateCommand = dbConnection.CreateCommand();
        tokenUpdateCommand.Parameters.Add(new MySqlParameter("tokenId",  tokenReader.GetInt32(0)));
        tokenUpdateCommand.CommandText = "UPDATE player_sso_tokens SET used_at = NOW() WHERE id = @tokenId LIMIT 1;";

        var playerId = tokenReader.GetInt32(1);
        
        await tokenReader.DisposeAsync();
        await tokenUpdateCommand.ExecuteNonQueryAsync();

        return playerId;
    }
    
    public static async Task<Player?> LoadPlayerAsync(
        SadieContext dbContext, 
        string token, 
        int authDelayMs)
    {
        var tokenExpires = DateTime
            .Now
            .Subtract(TimeSpan.FromMilliseconds(authDelayMs));

        await using var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();
        
        var playerId = await GetPlayerIdFromTokenAsync(connection, token, tokenExpires);

        if (playerId == 0)
        {
            return null;
        }
        
        var playerCommand = connection.CreateCommand();
        playerCommand.Parameters.Add(new MySqlParameter("id", playerId));
        playerCommand.CommandText = @"
            SELECT username, email, figure_code, home_room_id, credit_balance, pixel_balance, seasonal_balance, gotw_points, player_data.id FROM players 
                INNER JOIN player_data ON player_data.player_id = players.id
                INNER JOIN player_avatar_data ON player_avatar_data.player_id = players.id 
                INNER JOIN player_game_settings ON player_game_settings.player_id = players.id 
                INNER JOIN player_navigator_settings ON player_navigator_settings.player_id = players.id 
            WHERE players.id = @id";
        
        var playerReader = await playerCommand.ExecuteReaderAsync();

        if (!await playerReader.ReadAsync())
        {
            return null;
        }
        
        return new Player
        {
            Id = playerId,
            Username = playerReader.GetString(0),
            Email = playerReader.GetString(1),
    
            Data = new PlayerData
            {
                Id = playerReader.GetInt32(8),
                PlayerId = playerId,
                HomeRoomId = playerReader.IsDBNull(3) ? 0 : playerReader.GetInt32(3),
                CreditBalance = playerReader.GetInt32(4),
                PixelBalance = playerReader.GetInt32(5),
                SeasonalBalance = playerReader.GetInt32(6),
                GotwPoints = playerReader.GetInt32(7),
                RespectPoints = 0,
                RespectPointsPet = 0,
                AchievementScore = 0,
                AllowFriendRequests = false,
                IsOnline = false,
                LastOnline = DateTime.Now
            },
    
            AvatarData = new PlayerAvatarData
            {
                Id = 0,
                PlayerId = 0,
                Player = null,
                FigureCode = playerReader.GetString(2),
                Motto = "",
                Gender = AvatarGender.Male,
                ChatBubbleId = ChatBubble.Default
            },

            GameSettings = new PlayerGameSettings
            {
                Id = 0,
                PlayerId = 0,
                Player = null,
                SystemVolume = 0,
                FurnitureVolume = 0,
                TraxVolume = 0,
                PreferOldChat = false,
                BlockRoomInvites = false,
                BlockCameraFollow = false,
                UiFlags = 0,
                ShowNotifications = false
            },

            NavigatorSettings = new PlayerNavigatorSettings
            {
                Id = 0,
                PlayerId = 0,
                Player = null,
                WindowX = 0,
                WindowY = 0,
                WindowWidth = 0,
                WindowHeight = 0,
                OpenSearches = false,
                Unknown = 0
            },
            
            Rooms = [],
            Badges = [],
            Bots = [],
        };
    }
}