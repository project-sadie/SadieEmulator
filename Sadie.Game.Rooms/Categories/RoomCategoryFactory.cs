using Microsoft.Extensions.DependencyInjection;

namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryFactory
{
    private readonly IServiceProvider _serviceProvider;

    public RoomCategoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public RoomCategory CreateFromRecord(int id, string caption, bool isVisible)
    {
        return ActivatorUtilities.CreateInstance<RoomCategory>(_serviceProvider, id, caption, isVisible);
    }
}