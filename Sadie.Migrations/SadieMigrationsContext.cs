using Microsoft.EntityFrameworkCore;
using Sadie.Db;

namespace Sadie.Migrations;

public class SadieMigrationsContext() : SadieContext(CreateOptions())
{
    private static DbContextOptions<SadieContext> CreateOptions()
    {
        var connectionString = Environment.GetEnvironmentVariable("SADIE_CONNECTION_STRING");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Missing SADIE_CONNECTION_STRING environment variable.");
        }

        return new DbContextOptionsBuilder<SadieContext>()
            .UseMySql(
                connectionString,
                ServerVersion.AutoDetect(connectionString),
                b => b.MigrationsAssembly("Sadie.Migrations"))
            .UseSnakeCaseNamingConvention()
            .Options;
    }
}