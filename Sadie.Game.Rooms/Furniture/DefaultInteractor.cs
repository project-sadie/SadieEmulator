using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Players;

namespace Sadie.Game.Rooms.Furniture;

public class DefaultInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "default";
    
    public Task OnClickAsync(RoomLogic room, RoomFurnitureItem item, PlayerLogic player)
    {
        return Task.CompletedTask;
    }
}