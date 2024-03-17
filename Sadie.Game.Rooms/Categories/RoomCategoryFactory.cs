using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryFactory(IServiceProvider serviceProvider)
{
    public RoomCategory CreateFromRecord(int id, string caption, bool isVisible)
    {
        return ActivatorUtilities.CreateInstance<RoomCategory>(serviceProvider, id, caption, isVisible);
    }
}