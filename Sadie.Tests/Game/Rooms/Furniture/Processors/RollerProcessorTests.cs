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
    public async Task GetUpdatesForRoomAsync_NothingOnRoller_NoUpdatesReturned()
    {
        var room = MockRoomWithUserRepoAndFurniture("00", [
            MockFurnitureItemPlacementData(FurnitureItemInteractionType.Roller)
        ]);
        
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = await processor.GetUpdatesForRoomAsync(room);
        
        Assert.That(updates.Count(), Is.EqualTo(0));
    }
    
    [Test]
    public async Task GetUpdatesForRoomAsync_FurnitureOnRoller_PositionUpdated()
    {
        var roller = MockFurnitureItemPlacementData(FurnitureItemInteractionType.Roller);
        roller.Direction = HDirection.East;
        
        var roomItem1 = MockFurnitureItemPlacementData("");
        var room = MockRoomWithUserRepoAndFurniture("00\n\r00", [roller, roomItem1]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = await processor.GetUpdatesForRoomAsync(room);
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(1));
            Assert.That(roomItem1.PositionX, Is.EqualTo(1));
        });
    }
    
    [Test]
    public async Task GetUpdatesForRoomAsync_UserOnRoller_PositionUpdated()
    {
        var roller = MockFurnitureItemPlacementData(FurnitureItemInteractionType.Roller);
        roller.Direction = HDirection.East;
        
        var userOne = MockRoomUser();
        var room = MockRoomWithUserRepoAndFurniture("00\n\r00", [roller], [userOne]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = await processor.GetUpdatesForRoomAsync(room);
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(1));
            Assert.That(room.TileMap.UsersAtPoint(new Point(0,0)), Is.False);
        });
    }
    
    [Test]
    public async Task GetUpdatesForRoomAsync_UserOnRollerWithInvalidNextStep_NoUpdatesReturned()
    {
        var roller = MockFurnitureItemPlacementData(FurnitureItemInteractionType.Roller);
        
        roller.PositionX = 1;
        roller.PositionY = 1;
        var userOne = MockRoomUser();
        var room = MockRoomWithUserRepoAndFurniture("0", [roller], [userOne]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = await processor.GetUpdatesForRoomAsync(room);
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(0));
            Assert.That(room.TileMap.UsersAtPoint(new Point(0,0)), Is.True);
        });
    }

    [Test]
    public async Task GetUpdatesForRoomAsync_UserOnRollerWithUnwalkableNextStep_NoUpdatesReturned()
    {
        var roller = MockFurnitureItemPlacementData(FurnitureItemInteractionType.Roller);
        var item = MockFurnitureItemPlacementData("", 1, 1);
        var userOne = MockRoomUser();
        var room = MockRoomWithUserRepoAndFurniture("00", [roller, item], [userOne]);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = await processor.GetUpdatesForRoomAsync(room);
        
        Assert.Multiple(() =>
        {
            Assert.That(updates.Count(), Is.EqualTo(0));
            Assert.That(room.TileMap.UsersAtPoint(new Point(0,0)), Is.True);
        });
    }

    [Test]
    public async Task GetUpdatesForRoomAsync_FurnitureOnRollerWithInvalidNextStep_PositionNotUpdated()
    {
        var roller = MockFurnitureItemPlacementData(FurnitureItemInteractionType.Roller);
        var item = MockFurnitureItemPlacementData("");
        var room = MockRoomWithUserRepoAndFurniture("0", [roller, item], []);
        var tMapService = new RoomTileMapHelperService();
        var fHelperService = new RoomFurnitureItemHelperService();
        var processor = new RollerProcessor(tMapService, fHelperService);
        var updates = await processor.GetUpdatesForRoomAsync(room);
        
        Assert.Multiple(() =>
        {
            Assert.That(item.PositionX, Is.EqualTo(0));
            Assert.That(item.PositionY, Is.EqualTo(0));
        });
    }
}