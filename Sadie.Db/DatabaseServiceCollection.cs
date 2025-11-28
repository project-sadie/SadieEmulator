using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sadie.Db.Models.Catalog.FrontPage;
using Sadie.Db.Models.Constants;

namespace Sadie.Db;

public static class DatabaseServiceCollection
{
    public static void AddServices(IServiceCollection serviceCollection, IConfiguration config)
    {
        serviceCollection.AddDbContextFactory<SadieDbContext>(); 
        
        serviceCollection.AddDbContext<SadieDbContext>(options =>
        {
            options.UseMySql(config.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion,
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);

                        mySqlOptions.MigrationsAssembly("Sadie.Db");
                    })
                .LogTo(Console.WriteLine, LogLevel.Error);
        
            options.UseSnakeCaseNamingConvention();
        }, ServiceLifetime.Transient);
        
        serviceCollection.AddDbContextFactory<SadieMigrationsDbContext>();

        serviceCollection.AddSingleton<ServerPlayerConstants>(provider =>
            provider.GetRequiredService<SadieDbContext>()
                .ServerPlayerConstants
                .First()
        );

        serviceCollection.AddSingleton<ServerRoomConstants>(provider =>
            provider.GetRequiredService<SadieDbContext>()
                .ServerRoomConstants
                .OrderByDescending(x => x.CreatedAt)
                .First()
        );

        serviceCollection.AddSingleton(provider =>
            provider.GetRequiredService<SadieDbContext>()
                .Set<CatalogFrontPageItem>()
                .Include(x => x.CatalogPage)
                .ToList());
    }
}
