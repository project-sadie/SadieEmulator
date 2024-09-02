using System.Drawing;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Furniture.Processors;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Tests.Game.Rooms.Furniture.Processors;

public class RollerProcessorTests : RoomMockHelpers
{
    [Test]
    public void GetUpdatesForRoomAsync_NothingOnRoller_NoUpdatesReturned()
    {
        var room = MockRoomWithUserRepoAndFurniture("00", [
            MockPlacementData(FurnitureItemInteractionType.Roller)
        ]);
        
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = processor.GetUpdatesForRoomAsync(room).Result;
        
        Assert.That(updates.Count(), Is.EqualTo(0));
    }
    
    [Test]
    public void GetUpdatesForRoomAsync_FurnitureOnRoller_PositionUpdated()
    {
        var roller = MockPlacementData(FurnitureItemInteractionType.Roller);
        var roomItem1 = MockPlacementData("");
        var room = MockRoomWithUserRepoAndFurniture("00\n\r00", [roller, roomItem1]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = processor.GetUpdatesForRoomAsync(room).Result;
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(1));
            Assert.That(roomItem1.PositionY, Is.EqualTo(-1));
        });
    }
    
    [Test]
    public void GetUpdatesForRoomAsync_UserOnRoller_PositionUpdated()
    {
        var roller = MockPlacementData(FurnitureItemInteractionType.Roller);
        roller.Direction = HDirection.East;
        
        var userOne = MockRoomUser();
        var room = MockRoomWithUserRepoAndFurniture("00\n\r00", [roller], [userOne]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = processor.GetUpdatesForRoomAsync(room).Result;
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(1));
            Assert.That(room.TileMap.UsersAtPoint(new Point(0,0)), Is.False);
        });
    }
    
    [Test]
    public void GetUpdatesForRoomAsync_UserOnRollerWithInvalidNextStep_NoUpdatesReturned()
    {
        var roller = MockPlacementData(FurnitureItemInteractionType.Roller);
        
        roller.PositionX = 1;
        roller.PositionY = 1;
        var userOne = MockRoomUser();
        var room = MockRoomWithUserRepoAndFurniture("0", [roller], [userOne]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = processor.GetUpdatesForRoomAsync(room).Result;
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(0));
            Assert.That(room.TileMap.UsersAtPoint(new Point(0,0)), Is.True);
        });
    }

    [Test]
    public void GetUpdatesForRoomAsync_UserOnRollerWithUnwalkableNextStep_NoUpdatesReturned()
    {
        var roller = MockPlacementData(FurnitureItemInteractionType.Roller);
        var item = MockPlacementData("", 1, 1);
        var userOne = MockRoomUser();
        var room = MockRoomWithUserRepoAndFurniture("00", [roller, item], [userOne]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = processor.GetUpdatesForRoomAsync(room).Result;
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(0));
            Assert.That(room.TileMap.UsersAtPoint(new Point(0,0)), Is.True);
        });
    }
}