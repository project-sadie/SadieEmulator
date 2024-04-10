using Sadie.Database.Models.Rooms.Furniture;
using Sadie.Game.Players;

namespace Sadie.Game.Rooms.Furniture;

public interface IRoomFurnitureItemInteractor
{
    string InteractionType { get; }
    Task OnClickAsync(RoomLogic room, RoomFurnitureItem item, PlayerLogic player);
}