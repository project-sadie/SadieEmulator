using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Game.Navigator.Filterers;

namespace Sadie.Tests.Game.Navigator.Filterers;

public class OwnerFiltererTests : RoomMockHelpers
{
    private OwnerFilterer? _filterer;
    
    [SetUp]
    public void SetUp()
    {
        _filterer = new OwnerFilterer();
    }
    
    [Test]
    public void ApplyFilter_OneInMany_AppliedCorrectly()
    {
        var options = new DbContextOptionsBuilder<SadieDbContext>()
            .UseInMemoryDatabase(databaseName: "sadie")
            .Options;

        using var dbContext = new SadieDbContext(options);
        
        dbContext.Rooms.Add(MockRoomWithOwner("1"));
        dbContext.Rooms.Add(MockRoomWithOwner("2"));
        dbContext.Rooms.Add(MockRoomWithOwner("3"));

        dbContext.SaveChanges();
        
        var query = dbContext.Rooms.AsQueryable();
        var newQuery = _filterer!.Apply(query, "2");

        Assert.That(newQuery.ToList(), Has.Count.EqualTo(1));
    }
    
    [Test]
    public void ApplyFilter_ManyInMany_AppliedCorrectly()
    {
        var options = new DbContextOptionsBuilder<SadieDbContext>()
            .UseInMemoryDatabase(databaseName: "sadie")
            .Options;

        using var dbContext = new SadieDbContext(options);
        
        dbContext.Rooms.Add(MockRoomWithOwner("1"));
        dbContext.Rooms.Add(MockRoomWithOwner("1"));
        dbContext.Rooms.Add(MockRoomWithOwner("3"));

        dbContext.SaveChanges();
        
        var query = dbContext.Rooms.AsQueryable();
        var newQuery = _filterer!.Apply(query, "1");

        Assert.That(newQuery.ToList(), Has.Count.EqualTo(2));
    }
}