using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Shared.Unsorted.Game;
using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Players;

public static class PlayerLoader
{
    public static async Task<Player?> GetPlayerAsync(SadieContext dbContext, int authDelayMs)
    {
        var tokenExpires = DateTime
            .Now
            .Subtract(TimeSpan.FromMilliseconds(authDelayMs));
        
        await using var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();

        connection.CreateCommand();
        
        // TODO; Load player
        
        return new Player
        {
            Id = 1,
            Username = "habtard",
            Email = "",
    
            Data = new PlayerData
            {
                Id = 0,
                Player = null,
                PlayerId = 0,
                HomeRoomId = 0,
                CreditBalance = 0,
                PixelBalance = 0,
                SeasonalBalance = 0,
                GotwPoints = 0,
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
                FigureCode = "",
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
            }
        };
    }
}