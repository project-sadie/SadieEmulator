using System.Drawing;
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

    [Test]
    public void GetPointsForPlacement_SingleSquare_ReturnsCorrect()
    {
        var points = RoomTileMapHelpers.GetPointsForPlacement(10, 10, 1, 1, (int) HDirection.North);
        
        Assert.That(points, Has.Count.EqualTo(1));
        Assert.That(points.First(), Is.EqualTo(new Point(10, 10)));
    }

    [Test]
    public void GetPointsForPlacement_WideItem_ReturnsCorrect()
    {
        var points = RoomTileMapHelpers.GetPointsForPlacement(10, 10, 1, 5, (int) HDirection.North);
        
        Assert.Multiple(() =>
        {
            Assert.That(points, Has.Count.EqualTo(5));
            Assert.That(points[0], Is.EqualTo(new Point(10, 10)));
            Assert.That(points[1], Is.EqualTo(new Point(10, 11)));
            Assert.That(points[2], Is.EqualTo(new Point(10, 12)));
            Assert.That(points[3], Is.EqualTo(new Point(10, 13)));
            Assert.That(points[4], Is.EqualTo(new Point(10, 14)));
        });
    }

    [Test]
    public void GetPointsForPlacement_LongItem_ReturnsCorrect()
    {
        var points = RoomTileMapHelpers.GetPointsForPlacement(10, 10, 3, 1, (int) HDirection.North);
        
        Assert.That(points, Has.Count.EqualTo(3));
        
        Assert.Multiple(() =>
        {
            Assert.That(points[0], Is.EqualTo(new Point(10, 10)));
            Assert.That(points[1], Is.EqualTo(new Point(11, 10)));
            Assert.That(points[2], Is.EqualTo(new Point(12, 10)));
        });
    }

    [Test]
    public void GetStateNumberForTile_Open_ReturnsOpen()
    {
        var tileState = RoomTileMapHelpers.GetTileState(10, 10, new List<PlayerFurnitureItemPlacementData>());
        Assert.That(tileState, Is.EqualTo(RoomTileState.Open));
    }

    [Test]
    public void GetStateNumberForTile_Sit_ReturnsSit()
    {
        var tileState = RoomTileMapHelpers.GetTileState(10, 10, [
            new PlayerFurnitureItemPlacementData
            {
                PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        CanSit = true,
                        InteractionType = ""
                    },
                    LimitedData = "",
                    MetaData = ""
                },
                PositionX = 10,
                PositionY = 10
            }
        ]);
        
        Assert.That(tileState, Is.EqualTo(RoomTileState.Sit));
    }

    [Test]
    public void GetStateNumberForTile_Lay_ReturnsLay()
    {
        var tileState = RoomTileMapHelpers.GetTileState(10, 10, [
            new PlayerFurnitureItemPlacementData
            {
                PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        CanLay = true,
                        InteractionType = ""
                    },
                    LimitedData = "",
                    MetaData = ""
                },
                PositionX = 10,
                PositionY = 10
            }
        ]);
        
        Assert.That(tileState, Is.EqualTo(RoomTileState.Lay));
    }

    [Test]
    public void GetStateNumberForTile_GateInteractionType_ReturnsOpen()
    {
        var tileState = RoomTileMapHelpers.GetTileState(10, 10, [
            new PlayerFurnitureItemPlacementData
            {
                PlayerFurnitureItem = new PlayerFurnitureItem
                {
                    FurnitureItem = new FurnitureItem
                    {
                        CanWalk = false,
                        InteractionType = "gate"
                    },
                    LimitedData = "",
                    MetaData = "1"
                },
                PositionX = 10,
                PositionY = 10
            }
        ]);
        
        Assert.That(tileState, Is.EqualTo(RoomTileState.Open));
    }
    
    [Test]
    public void GetItemsForPosition_SingleItem_ReturnsCorrect()
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
    public void GetWorldArrayFromTileMap_ChairNotGoal_ReturnsUnwalkable()
    {
    }
    
    [Test]
    public void GetWorldArrayFromTileMap_BedNotGoal_ReturnsUnwalkable()
    {
    }
    
    [Test]
    public void GetWorldArrayFromTileMap_TilesWithBlockingFurniture_ReturnsUnwalkable()
    {
    }
    
    [Test]
    public void GetWorldArrayFromTileMap_TilesWithNonBlockingFurniture_ReturnsWalkable()
    {
    }
    
    [Test]
    public void GetWorldArrayFromTileMap_TilesWithUsers_ReturnsUnwalkable()
    {
    }
    
    [Test]
    public void GetWorldArrayFromTileMap_EmptyTiles_ReturnsWalkable()
    {
    }
    
    [Test]
    public void GetWorldArrayFromTileMap_TileWithBlockingFurnitureButCanOverride_ReturnsWalkable()
    {
    }
    
    [Test]
    public void CanPlaceAt_OpenPoint_ReturnsTrue()
    {
    }
    
    [Test]
    public void CanPlaceAt_OpenPoints_ReturnsTrue()
    {
    }
    
    [Test]
    public void CanPlaceAt_ClosedPoint_ReturnsFalse()
    {
    }
    
    [Test]
    public void CanPlaceAt_OpenClosedPoints_ReturnsFalse()
    {
    }
    
    [Test]
    public void GetUsersAtPoints_PointWithUser_ReturnsCorrect()
    {
    }
    
    [Test]
    public void GetPointInFront_North_ReturnsCorrect()
    {
    }
    
    [Test]
    public void GetPointInFront_East_ReturnsCorrect()
    {
    }
    
    [Test]
    public void GetPointInFront_South_ReturnsCorrect()
    {
    }
    
    [Test]
    public void GetPointInFront_West_ReturnsCorrect()
    {
    }
    
    [Test]
    public void GetPointInFront_NorthWithOffset_ReturnsCorrect()
    {
    }
    
    // TODO; Test GetItemPlacementHeight
    // TODO; Test GetSquaresBetweenPoints
    // TODO; Test GetEffectFromInteractionType
}