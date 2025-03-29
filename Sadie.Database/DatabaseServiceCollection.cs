using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Database.Models.Constants;

namespace Sadie.Database;

public static class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddDbContextFactory<SadieContext>(); 
        
        serviceCollection.AddDbContext<SadieContext>(options =>
        {
            options.UseMySql(config.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion, mySqlOptions =>
            {
                mySqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 10,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });
        
            options.UseSnakeCaseNamingConvention();
        }, ServiceLifetime.Transient);

        serviceCollection.AddSingleton<ServerPlayerConstants>(provider =>
            provider.GetRequiredService<SadieContext>()
                .ServerPlayerConstants
                .First()
            );

        serviceCollection.AddSingleton<ServerRoomConstants>(provider =>
            provider.GetRequiredService<SadieContext>()
                .ServerRoomConstants
                .OrderByDescending(x => x.CreatedAt)
                .First()
            );

        serviceCollection.AddSingleton(provider =>
            provider.GetRequiredService<SadieContext>()
                .Set<CatalogPage>()
                .Include("Pages")
                .Include("Pages.Pages")
                .Include("Pages.Pages.Pages")
                .Include("Pages.Pages.Pages.Pages")
                .Include(c => c.Items).ThenInclude(x => x.FurnitureItems)
                .ToList());

        serviceCollection.AddSingleton(provider =>
            provider.GetRequiredService<SadieContext>()
                .Set<CatalogFrontPageItem>()
                .Include(x => x.CatalogPage)
                .ToList());
    }
}