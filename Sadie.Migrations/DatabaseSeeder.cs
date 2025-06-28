using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Db.Models.Constants;
using Sadie.Db.Models.Navigator;
using Sadie.Db.Models.Server;

namespace Sadie.Migrations;

public static class DatabaseSeeder
{
    public static async Task SeedInitialDataAsync(SadieContext context)
    {
        await SeedServerSettingsAsync(context);
        await SeedConstantsAsync(context);
        await SeedNavigatorAsync(context);
        
        // already got the others as raw sql files so just source them, cba
        
        var rawSqlFiles = Directory.EnumerateFiles("Seeders", "*.sql");

        foreach (var file in rawSqlFiles)
        {
            var sql = await File.ReadAllTextAsync(file);
            await context.Database.ExecuteSqlRawAsync(sql);
        }
    }

    private static async Task SeedServerSettingsAsync(SadieContext context)
    {
        if (!context.ServerSettings.Any())
        {
            context.ServerSettings.Add(new ServerSettings
            {
                PlayerWelcomeMessage = "Welcome (back) to Sadie [username], we're running version [version]!",
                FairCurrencyRewards = true
            });
        }
        
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedConstantsAsync(SadieContext context)
    {
        if (!context.ServerPlayerConstants.Any())
        {
            context.ServerPlayerConstants.Add(new ServerPlayerConstants
            {
                MaxMottoLength = 35,
                MinSsoLength = 8,
                MaxFriendships = 20000
            });
        }

        if (!context.ServerRoomConstants.Any())
        {
            context.ServerRoomConstants.Add(new ServerRoomConstants
            {
                MaxChatMessageLength = 500,
                SecondsTillUserIdle = 300,
                MaxNameLength = 60,
                MaxDescriptionLength = 250,
                MaxTagLength = 30,
                WiredMaxFurnitureSelection = 5,
                CreatedAt = DateTime.Now
            });
        }

        await context.SaveChangesAsync();
    }

    private static async Task SeedNavigatorAsync(SadieContext context)
    {
        if (!context.NavigatorTabs.Any())
        {
            context.NavigatorTabs.AddRange(
                new NavigatorTab { Id = 1, Name = "query" },
                new NavigatorTab { Id = 2, Name = "official_view" },
                new NavigatorTab { Id = 3, Name = "hotel_view" },
                new NavigatorTab { Id = 4, Name = "myworld_view" },
                new NavigatorTab { Id = 5, Name = "roomads_view" }
            );
        }

        if (!context.NavigatorCategories.Any())
        {
            context.NavigatorCategories.AddRange(
                new NavigatorCategory { Id = 1, Name = "Most Popular Rooms", CodeName = "popular", TabId = 3, OrderId = 0 },
                new NavigatorCategory { Id = 2, Name = "My Rooms", CodeName = "my_rooms", TabId = 4, OrderId = 0 }
            );
        }

        await context.SaveChangesAsync();
    }
}