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
        
        var baseDir = AppContext.BaseDirectory;
        var seedersPath = Path.Combine(baseDir, "..", "..", "..", "Sadie.Migrations", "Seeders");

        var rawSqlFiles = Directory.EnumerateFiles(seedersPath, "*.sql");

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
            var sql = @"
                INSERT INTO server_settings (player_welcome_message, fair_currency_rewards) 
                VALUES ('Welcome (back) to Sadie [username], we''re running version [version]!', true);
            ";

            await context.Database.ExecuteSqlRawAsync(sql);
        }
        
        await context.SaveChangesAsync();
    }
    
    private static async Task SeedConstantsAsync(SadieContext context)
    {
        if (!context.ServerPlayerConstants.Any())
        {
            var playerConstantsSql = @"
                INSERT INTO server_player_constants (max_motto_length, min_sso_length, max_friendships)
                VALUES (35, 8, 20000);
            ";
            await context.Database.ExecuteSqlRawAsync(playerConstantsSql);
        }

        if (!context.ServerRoomConstants.Any())
        {
            var insertRoomConstantsSql = @"
                INSERT INTO server_room_constants
                    (max_chat_message_length, seconds_till_user_idle, max_name_length, max_description_length, max_tag_length, wired_max_furniture_selection, created_at)
                VALUES
                    (500, 300, 60, 250, 30, 5, NOW());
            ";
            await context.Database.ExecuteSqlRawAsync(insertRoomConstantsSql);
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