using Sadie.Game.Rooms;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Tests.Rooms;

[TestFixture]
public class RoomHelperTests
{
    [Test]
    public void GetItemsForPosition_ReturnsCorrect()
    {
        // TODO;
        RoomHelpers.GetItemsForPosition(4, 17)
    }
    
    [Test]
    public void GetOppositeDirection_ReturnsCorrect()
    {
        var northResult = RoomHelpers.GetOppositeDirection((int) HDirection.North);
        var northEastResult = RoomHelpers.GetOppositeDirection((int) HDirection.NorthEast);
        var eastResult = RoomHelpers.GetOppositeDirection((int) HDirection.East);
        var southEastResult = RoomHelpers.GetOppositeDirection((int) HDirection.SouthEast);
        var southResult = RoomHelpers.GetOppositeDirection((int) HDirection.South);
        var southWestResult = RoomHelpers.GetOppositeDirection((int) HDirection.SouthWest);
        var westResult = RoomHelpers.GetOppositeDirection((int) HDirection.West);
        var northWestResult = RoomHelpers.GetOppositeDirection((int) HDirection.NorthWest);
        
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