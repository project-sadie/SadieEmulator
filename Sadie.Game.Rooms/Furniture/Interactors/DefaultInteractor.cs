using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Interactors;

public class DefaultInteractor : IRoomFurnitureItemInteractor
{
    public string InteractionType => "default";
    
    public Task OnClickAsync(RoomLogic room, RoomFurnitureItem item, IRoomUser roomUser)
    {
        return Task.CompletedTask;
    }
}