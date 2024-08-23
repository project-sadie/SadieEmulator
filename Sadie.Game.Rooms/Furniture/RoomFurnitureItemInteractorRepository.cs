using Sadie.API.Game.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture;

public class RoomFurnitureItemInteractorRepository(
    IEnumerable<IRoomFurnitureItemInteractor> interactors)
{
    private readonly Dictionary<List<string>, IRoomFurnitureItemInteractor> _interactors = 
        interactors.ToDictionary(x => x.InteractionTypes, x => x);

    public ICollection<IRoomFurnitureItemInteractor> GetInteractorsForType(string interactionType) =>
        _interactors
            .Values
            .Where(x => x.InteractionTypes.Contains(interactionType))
            .ToList();
}