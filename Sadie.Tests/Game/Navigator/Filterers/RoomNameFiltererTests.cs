using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Game.Navigator.Filterers;

namespace Sadie.Tests.Game.Navigator.Filterers;

public class RoomNameFiltererTests : RoomMockHelpers
{
    private INavigatorSearchFilterer? _filterer;
    
    [SetUp]
    public void SetUp()
    {
        _filterer = new RoomNameFilterer();
    }
    
    [Test]
    public void ApplyFilter_OneInMany_AppliedCorrectly()
    {
        var options = new DbContextOptionsBuilder<SadieContext>()
            .UseInMemoryDatabase(databaseName: "sadie")
            .Options;

        using var dbContext = new SadieContext(options);
        
        dbContext.Rooms.Add(MockRoomWithName("someName1"));
        dbContext.Rooms.Add(MockRoomWithName("someName2"));
        dbContext.Rooms.Add(MockRoomWithName("someName3"));

        dbContext.SaveChanges();
        
        var query = dbContext.Rooms.AsQueryable();
        var newQuery = _filterer!.ApplyFilter(query, "someName2");

        Assert.That(newQuery.ToList(), Has.Count.EqualTo(1));
    }
}