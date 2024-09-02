using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Services;

namespace Sadie.Tests.Game.Rooms.Services;

public class RoomWiredServiceTests : RoomMockHelpers
{
    [Test]
    public void GetEffectsForTrigger_TriggersInStack_ReturnsJustEffects()
    {
        var furnitureItemHelperService = new RoomFurnitureItemHelperService();
        var wiredService = new RoomWiredService(furnitureItemHelperService);
        var trigger = MockPlacementData(FurnitureItemInteractionType.WiredTriggerEnterRoom);
        
        var items = new List<PlayerFurnitureItemPlacementData>
        {
            trigger,
            MockPlacementData(FurnitureItemInteractionType.WiredEffectShowMessage, 0, 0, 1),
            MockPlacementData(FurnitureItemInteractionType.WiredEffectKickUser, 0, 0, 2),
            MockPlacementData(FurnitureItemInteractionType.WiredEffectShowMessage, 0, 0, 4),
            MockPlacementData(FurnitureItemInteractionType.WiredTriggerEnterRoom, 0, 0, 3),
            MockPlacementData(FurnitureItemInteractionType.WiredEffectShowMessage, 0, 0, 4),
        };

        var effects = wiredService.GetEffectsForTrigger(trigger, items);
        
        Assert.That(effects.Count, Is.EqualTo(2));
    }
}