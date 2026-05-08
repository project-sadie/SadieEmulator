using System.Drawing;
using Moq;
using Sadie.API.DTOs.Furniture;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Unit;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Tests.Game.Rooms.Mapping;

[TestFixture]
public class RoomTileMapTests
{
    [Test]
    public void UpdateEffectMapForTile_SwimTiles_WorksAsExpected()
    {
        var waterItem = MockFurnitureItemPlacementData(FurnitureItemInteractionType.Water);
        var tileMap = new RoomTileMap("0", [waterItem]);

        Assert.That(tileMap.EffectMap[0, 0], Is.EqualTo(29));
    }

    [Test]
    public void AddUserToMap_AddUser_IncrementsMapCount()
    {
        var tileMap = new RoomTileMap("0", []);
        var unit = new Mock<IRoomUnitData>().Object;
        var point = new Point(0, 0);

        tileMap.AddUnitToMap(point, unit);
        Assert.Multiple(() =>
        {
            Assert.That(tileMap.UnitMap.ContainsKey(point), Is.True);
            Assert.That(tileMap.UnitMap[point], Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void AddBotToMap_AddBot_IncrementsMapCount()
    {
        var tileMap = new RoomTileMap("0", []);
        var bot = new Mock<IRoomUnitData>().Object;
        var point = new Point(0, 0);

        tileMap.AddUnitToMap(point, bot);
        Assert.Multiple(() =>
        {
            Assert.That(tileMap.UnitMap.ContainsKey(point), Is.True);
            Assert.That(tileMap.UnitMap[point], Has.Count.EqualTo(1));
        });
    }

    [Test]
    public void UsersAtPoint_PopulatedPoint_ReturnsTrue()
    {
        var tileMap = new RoomTileMap("0", []);
        var unit = new Mock<IRoomUnitData>().Object;
        var point = new Point(0, 0);

        tileMap.AddUnitToMap(point, unit);

        Assert.That(tileMap.UsersAtPoint(point), Is.True);
    }

    [Test]
    public void UsersAtPoint_EmptyPoint_ReturnsFalse()
    {
        var tileMap = new RoomTileMap("0", []);

        Assert.That(tileMap.UsersAtPoint(new Point(0, 0)), Is.False);
    }

    [TestCase("0", 1, 3, false)]
    [TestCase("0", 0, 0, true)]
    public void TileExists_ReturnsCorrect(string heightMap, int x, int y, bool idk)
    {
        var tileMap = new RoomTileMap(heightMap, []);
        Assert.That(tileMap.TileExists(new Point(x, y)), Is.EqualTo(idk));
    }

    private static PlayerFurnitureItemPlacementDataDto MockFurnitureItemPlacementData(
        string interactionType, int x = 0, int y = 0, int z = 0) =>
        new()
        {
            PlayerFurnitureItem = new PlayerFurnitureItemDto
            {
                FurnitureItemId = 0,
                FurnitureItem = new FurnitureItemDto
                {
                    InteractionType = interactionType,
                    CanWalk = false,
                    Name = "",
                    AssetName = "",
                    Type = FurnitureItemType.Floor,
                    TileSpanX = 1,
                    TileSpanY = 1
                },
                LimitedData = "",
                MetaData = ""
            },
            PositionX = x,
            PositionY = y,
            PositionZ = z
        };
}
