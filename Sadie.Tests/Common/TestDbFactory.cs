using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sadie.Db;

namespace Sadie.Tests.Common;

public class TestDbFactory
{
    public static IDbContextFactory<SadieDbContext> CreateDbFactory()
    {
        var options = new DbContextOptionsBuilder<SadieDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var factory = new PooledDbContextFactory<SadieDbContext>(options);
        return factory;
    }
}