using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DimmerInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "dimmer";
    
    public async Task OnTriggerAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
    {
    }

    public async Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
    {
    }

    public async Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
    {
    }

    public Task OnStepOnAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit? roomUnit) => Task.CompletedTask;
    public Task OnStepOffAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit? roomUnit) => Task.CompletedTask;
}