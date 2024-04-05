using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Rooms.Categories;

public class RoomCategoryRepository(SadieContext dbContext)
{
    private List<RoomCategory> _categories = [];

    public async Task LoadInitialDataAsync()
    {
        _categories = await dbContext.Set<RoomCategory>().ToListAsync();
    }

    public List<RoomCategory> GetAllCategories()
    {
        return _categories;
    }
}