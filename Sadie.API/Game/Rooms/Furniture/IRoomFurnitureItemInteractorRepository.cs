namespace Sadie.API.Game.Rooms.Furniture;

public interface IRoomFurnitureItemInteractorRepository
{
    ICollection<IRoomFurnitureItemInteractor> GetInteractorsForType(string interactionType);
}