using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class RollerInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "roller";

    public Task OnTriggerAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
    {
        Console.WriteLine("Roller triggered");
        return Task.CompletedTask;
    }

    public Task OnPlaceAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
    {
        Console.WriteLine("Roller placed");
        return Task.CompletedTask;
    }

    public Task OnMoveAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
    {
        Console.WriteLine("Roller moved");
        return Task.CompletedTask;
    }

    public Task OnStepOnAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit)
    {
        Console.WriteLine("Roller walked on");
        return Task.CompletedTask;
    }
    
    public Task OnStepOffAsync(IRoomLogic room, RoomFurnitureItem item, IRoomUnit roomUnit) 
    {
        Console.WriteLine("Roller walked off");
        return Task.CompletedTask;
    }
}