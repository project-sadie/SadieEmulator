using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Navigator;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(RoomRepository roomRepository, SadieContext dbContext) : INavigatorRoomProvider
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
            .Include(x => x.Owner)
            .Include(x => x.Settings)
            .Include(x => x.Tags);
        
        if (searchQuery.Contains(":"))
        {
            var searchQueryParts = searchQuery.Split(":");
            var filterName = searchQueryParts[0];
            var filterValue = searchQuery[(filterName.Length + 1)..];
        
            return filterName switch
            {
                "roomname" => await query
                    .Where(x => x.Name.Contains(filterValue))
                    .ToListAsync(),
            
                "owner" => await query
                    .Where(x => x.Owner!.Username.Contains(filterValue))
                    .ToListAsync(),
            
                "tag" => await query
                    .Where(x => x.Tags.Any(x => x.Name.Contains(filterValue)))
                    .ToListAsync(),
            
                _ => []
            };
        }

        return await query
            .Where(x => 
                x.Name.Contains(searchQuery) ||
                x.Description.Contains(searchQuery) ||
                x.Tags.Any(t => t.Name.Contains(searchQuery)) ||
                x.Owner!.Username.Contains(searchQuery))
            .ToListAsync();
    }
}