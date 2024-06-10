using Sadie.API.Game.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture;

public class RoomFurnitureItemInteractorRepository(
    IEnumerable<IRoomFurnitureItemInteractor> interactors)
{
    private readonly Dictionary<string, IRoomFurnitureItemInteractor> _interactors = 
        interactors.ToDictionary(x => x.InteractionType, x => x);

    public IRoomFurnitureItemInteractor? GetInteractorForType(string interactionType) =>
        _interactors.GetValueOrDefault(interactionType);
}