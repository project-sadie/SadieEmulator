using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.API.Game.Rooms.Wired.Effects;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Shared.Extensions;

namespace Sadie.Game.Rooms.Wired.Effects;

public class TeleportToFurnitureEffectRunner : IWiredEffectRunner
{
    public async Task ExecuteAsync(
        IRoomLogic room, 
        IRoomUser userWhoTriggered, 
        PlayerFurnitureItemPlacementData effect)
    {
        var placementDataId = effect.WiredData!.PlayerFurnitureItemWiredItems.PickRandom()
            .PlayerFurnitureItemPlacementDataId;
        
        var item = room
            .FurnitureItems
            .First(x => x.Id == placementDataId);
        
        var newPoint = new Point(item.PositionX, item.PositionY);

        room.TileMap.UnitMap[userWhoTriggered.Point].Remove(userWhoTriggered);
        room.TileMap.AddUnitToMap(newPoint, userWhoTriggered);

        await userWhoTriggered.SetPositionAsync(newPoint);
        
        userWhoTriggered.CheckStatusForCurrentTile();
    }
}