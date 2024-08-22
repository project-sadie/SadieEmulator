using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Navigator;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Navigator.Filterers;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(
    IRoomRepository roomRepository, 
    SadieContext dbContext,
    IEnumerable<INavigatorSearchFilterer> filterers) : INavigatorRoomProvider
{
    public Task<List<Room>> GetRoomsForCategoryNameAsync(IPlayerLogic player, string category)
    {
        return Task.FromResult(category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            "my_rooms" => player.Rooms.ToList(),
            _ => []
        });
    }

    public async Task<List<Room>> GetRoomsForSearchQueryAsync(string searchQuery)
    {
        var query = dbContext
            .Rooms
            .AsQueryable();
        
        if (searchQuery.Contains(':'))
        {
            query = ApplyFilter(query, searchQuery.Split([':'], 2));
        }

        return await query
            .Where(x => 
                x.Name.Contains(searchQuery) ||
                x.Description.Contains(searchQuery) ||
                x.Tags.Any(t => t.Name.Contains(searchQuery)) ||
                x.Owner!.Username.Contains(searchQuery))
            .ToListAsync();
    }

    private IQueryable<Room> ApplyFilter(IQueryable<Room> query, IReadOnlyList<string> filterData)
    {
        var filterer = filterers.FirstOrDefault(x => x.Name == filterData[0]);

        return filterer != null ? 
            filterer.ApplyFilter(query, filterData[1]) : 
            query;
    }
}