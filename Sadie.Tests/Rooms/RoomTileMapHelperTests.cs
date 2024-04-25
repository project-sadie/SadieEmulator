using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Tests.Rooms;

[TestFixture]
public class RoomTileMapHelperTests
{
    [Test]
    public void GetItemsForPosition_ReturnsCorrect()
    {
        var someItems = new List<RoomFurnitureItem>
        {
            new() { PositionX = 10, PositionY = 14, FurnitureItem = new FurnitureItem { Type = FurnitureItemType.Floor } }
        };
        
        Assert.That(RoomTileMapHelpers.GetItemsForPosition(10, 14, someItems), Has.Count.EqualTo(1));
    }
    
    [Test]
    public void GetItemsForPosition_NotFound_ReturnsCorrect()
    {
        var someItems = new List<RoomFurnitureItem>
        {
            new() { PositionX = 10, PositionY = 14, FurnitureItem = new FurnitureItem { Type = FurnitureItemType.Floor } }
        };
        
        Assert.That(RoomTileMapHelpers.GetItemsForPosition(14, 10, someItems), Is.Empty);
    }
    
    [Test]
    public void GetItemsForPosition_ReturnsCorrectForBulk()
    {
        var someItems = new List<RoomFurnitureItem>
        {
            new() { PositionX = 10, PositionY = 14, FurnitureItem = new FurnitureItem { Type = FurnitureItemType.Floor } },
            new() { PositionX = 4, PositionY = 5, FurnitureItem = new FurnitureItem { Type = FurnitureItemType.Floor } },
            new() { PositionX = 10, PositionY = 14, FurnitureItem = new FurnitureItem { Type = FurnitureItemType.Floor } },
            new() { PositionX = 10, PositionY = 14, FurnitureItem = new FurnitureItem { Type = FurnitureItemType.Floor } },
        };
        
        Assert.That(RoomTileMapHelpers.GetItemsForPosition(10, 14, someItems), Has.Count.EqualTo(3));
    }
    
    [Test]
    public void GetOppositeDirection_ReturnsCorrect()
    {
        var northResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.North);
        var northEastResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.NorthEast);
        var eastResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.East);
        var southEastResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.SouthEast);
        var southResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.South);
        var southWestResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.SouthWest);
        var westResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.West);
        var northWestResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.NorthWest);
        
        Assert.Multiple(() =>
        {
            Assert.That(northResult, Is.EqualTo(HDirection.South));
            Assert.That(northEastResult, Is.EqualTo(HDirection.SouthWest));
            Assert.That(eastResult, Is.EqualTo(HDirection.West));
            Assert.That(southEastResult, Is.EqualTo(HDirection.NorthWest));
            Assert.That(southResult, Is.EqualTo(HDirection.North));
            Assert.That(southWestResult, Is.EqualTo(HDirection.NorthEast));
            Assert.That(westResult, Is.EqualTo(HDirection.East));
            Assert.That(northWestResult, Is.EqualTo(HDirection.SouthEast));
        });
    }
}