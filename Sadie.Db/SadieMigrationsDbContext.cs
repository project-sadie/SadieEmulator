using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Sadie.Db;

public class SadieMigrationsDbContext(DbContextOptions<SadieDbContext> options) : SadieDbContext(options);

public class SadieMigrationsContextFactory : IDesignTimeDbContextFactory<SadieMigrationsDbContext>
{
    public SadieMigrationsDbContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("Default");
        
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        return new SadieMigrationsDbContext(new DbContextOptionsBuilder<SadieDbContext>()
            .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                b => b.MigrationsAssembly("Sadie.Db"))
            .UseSnakeCaseNamingConvention()
            .Options);
    }
}