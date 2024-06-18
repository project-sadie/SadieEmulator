using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Players.Furniture;

namespace Sadie.API.Game.Rooms.Furniture;

public abstract class AbstractRoomFurnitureItemInteractor : IRoomFurnitureItemInteractor
{
    public abstract string InteractionType { get; }
    
    public virtual Task OnTriggerAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPlaceAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnPickUpAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        return Task.CompletedTask;
    }

    public virtual Task OnMoveAsync(IRoomLogic room, PlayerFurnitureItemPlacementData item, IRoomUnit roomUnit)
    {
        return Task.CompletedTask;
    }
}