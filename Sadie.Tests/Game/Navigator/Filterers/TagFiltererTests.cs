using Microsoft.EntityFrameworkCore;
using Sadie.Db;
using Sadie.Game.Navigator.Filterers;

namespace Sadie.Tests.Game.Navigator.Filterers;

public class TagFiltererTests : RoomMockHelpers
{
    private TagFilterer? _filterer;
    
    [SetUp]
    public void SetUp()
    {
        _filterer = new TagFilterer();
    }
    
    [Test]
    public void ApplyFilter_NoMatches_AppliedCorrectly()
    {
        var options = new DbContextOptionsBuilder<SadieContext>()
            .UseInMemoryDatabase(databaseName: "sadie")
            .Options;
        
        using var dbContext = new SadieContext(options);
        
        var query = dbContext.Rooms.AsQueryable();
        var newQuery = _filterer!.Apply(query, "match");
        
        Assert.That(newQuery.ToList(), Has.Count.EqualTo(0));
    }
    
    [Test]
    public void ApplyFilter_OneInMany_AppliedCorrectly()
    {
        var options = new DbContextOptionsBuilder<SadieContext>()
            .UseInMemoryDatabase(databaseName: "sadie")
            .Options;

        using var dbContext = new SadieContext(options);
        
        dbContext.Rooms.Add(MockRoomWithTag("1"));
        dbContext.Rooms.Add(MockRoomWithTag("2"));
        dbContext.Rooms.Add(MockRoomWithTag("3"));

        dbContext.SaveChanges();
        
        var query = dbContext.Rooms.AsQueryable();
        var newQuery = _filterer!.Apply(query, "2");

        Assert.That(newQuery.ToList(), Has.Count.EqualTo(1));
    }
    
    [Test]
    public void ApplyFilter_ManyInMany_AppliedCorrectly()
    {
        var options = new DbContextOptionsBuilder<SadieContext>()
            .UseInMemoryDatabase(databaseName: "sadie")
            .Options;

        using var dbContext = new SadieContext(options);
        
        dbContext.Rooms.Add(MockRoomWithTag("1"));
        dbContext.Rooms.Add(MockRoomWithTag("1"));
        dbContext.Rooms.Add(MockRoomWithTag("3"));

        dbContext.SaveChanges();
        
        var query = dbContext.Rooms.AsQueryable();
        var newQuery = _filterer!.Apply(query, "1");

        Assert.That(newQuery.ToList(), Has.Count.EqualTo(2));
    }
}