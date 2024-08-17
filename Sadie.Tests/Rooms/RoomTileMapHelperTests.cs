using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Tests.Rooms;

[TestFixture]
public class RoomTileMapHelperTests
{
    [Test]
    public void GetItemsForPosition_ReturnsCorrect()
    {
        var someItems = new List<PlayerFurnitureItemPlacementData>
        {
            new() { PositionX = 10, PositionY = 14, PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        Type = FurnitureItemType.Floor,
                        InteractionType = "default"
                    },
                    
                    LimitedData = "",
                    MetaData = ""
                }
            }
        };
        
        Assert.That(RoomTileMapHelpers.GetItemsForPosition(10, 14, someItems), Has.Count.EqualTo(1));
    }
    
    [Test]
    public void GetItemsForPosition_NotFound_ReturnsCorrect()
    {
        var someItems = new List<PlayerFurnitureItemPlacementData>
        {
            new() { PositionX = 10, PositionY = 14, PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        Type = FurnitureItemType.Floor,
                        InteractionType = "default"
                    },
                    LimitedData = "",
                    MetaData = ""
                }
            }
        };
        
        Assert.That(RoomTileMapHelpers.GetItemsForPosition(14, 10, someItems), Is.Empty);
    }
    
    [Test]
    public void GetItemsForPosition_Bulk_ReturnsCorrect()
    {
        var someItems = new List<PlayerFurnitureItemPlacementData>
        {
            new() { PositionX = 10, PositionY = 14, PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        Type = FurnitureItemType.Floor,
                        InteractionType = "default"
                    },
                    LimitedData = "",
                    MetaData = ""
                }
            },
            new() { PositionX = 4, PositionY = 5, PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        Type = FurnitureItemType.Floor,
                        InteractionType = "default"
                    },
                    LimitedData = "",
                    MetaData = ""
                }
            },
            new() { PositionX = 10, PositionY = 14, PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        Type = FurnitureItemType.Floor,
                        InteractionType = "default"
                    },
                    LimitedData = "",
                    MetaData = ""
                }
            },
            new() { PositionX = 10, PositionY = 14, PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        Type = FurnitureItemType.Floor,
                        InteractionType = "default"
                    },
                    LimitedData = "",
                    MetaData = ""
                }
            },
        };
        
        Assert.That(RoomTileMapHelpers.GetItemsForPosition(10, 14, someItems), Has.Count.EqualTo(3));
    }
    
    [Test]
    public void GetOppositeDirection_North_ReturnsCorrect()
    {
        var northResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.North);
        Assert.That(northResult, Is.EqualTo(HDirection.South));
    }
    
    [Test]
    public void GetOppositeDirection_NorthEast_ReturnsCorrect()
    {
        var northEastResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.NorthEast);
        Assert.That(northEastResult, Is.EqualTo(HDirection.SouthWest));
    }
    
    [Test]
    public void GetOppositeDirection_East_ReturnsCorrect()
    {
        var eastResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.East);
        Assert.That(eastResult, Is.EqualTo(HDirection.West));
    }
    
    [Test]
    public void GetOppositeDirection_SouthEast_ReturnsCorrect()
    {
        var southEastResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.SouthEast);
        Assert.That(southEastResult, Is.EqualTo(HDirection.NorthWest));
    }
    
    [Test]
    public void GetOppositeDirection_South_ReturnsCorrect()
    {
        var southResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.South);
        Assert.That(southResult, Is.EqualTo(HDirection.North));
    }
    
    [Test]
    public void GetOppositeDirection_SouthWest_ReturnsCorrect()
    {
        var southWestResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.SouthWest);
        Assert.That(southWestResult, Is.EqualTo(HDirection.NorthEast));
    }
    
    [Test]
    public void GetOppositeDirection_West_ReturnsCorrect()
    {
        var westResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.West);
        Assert.That(westResult, Is.EqualTo(HDirection.East));
    }
    
    [Test]
    public void GetOppositeDirection_NorthWest_ReturnsCorrect()
    {
        var northWestResult = RoomTileMapHelpers.GetOppositeDirection((int) HDirection.NorthWest);
        Assert.That(northWestResult, Is.EqualTo(HDirection.SouthEast));
    }
}