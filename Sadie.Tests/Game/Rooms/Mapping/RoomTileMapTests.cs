using System.Drawing;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Tests.Game.Rooms.Mapping;

[TestFixture]
public class RoomTileMapTests
{
    [Test]
    public void UpdateEffectMapForTile_SwimTiles_WorksAsExpected()
    {
    }
    
    [Test]
    public void AddUserToMap_AddUser_IncrementsMapCount()
    {
    }
    
    [Test]
    public void AddBotToMap_AddBot_IncrementsMapCount()
    {
    }
    
    [Test]
    public void UsersAtPoint_PopulatedPoint_ReturnsTrue()
    {
    }
    
    [Test]
    public void UsersAtPoint_EmptyPoint_ReturnsFalse()
    {
    }
    
    [TestCase("0", 1, 3, false)]
    [TestCase("0", 0, 0, true)]
    public void TileExists_ReturnsCorrect(string heightMap, int x, int y, bool idk)
    {
        var tileMap = new RoomTileMap(heightMap, []);
        Assert.That(tileMap.TileExists(new Point(x, y)), Is.EqualTo(idk));
    }
}