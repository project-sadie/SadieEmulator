using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Sadie.Db;

namespace Sadie.Migrations;

public class SadieMigrationsContext : SadieContext
{
    public SadieMigrationsContext(DbContextOptions<SadieContext> options) : base(options)
    {
    }
}

public class SadieMigrationsContextFactory : IDesignTimeDbContextFactory<SadieMigrationsContext>
{
    public SadieMigrationsContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = config.GetConnectionString("Default");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        return new SadieMigrationsContext(new DbContextOptionsBuilder<SadieContext>()
            .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                b => b.MigrationsAssembly("Sadie.Migrations"))
            .UseSnakeCaseNamingConvention()
            .Options);
    }
}