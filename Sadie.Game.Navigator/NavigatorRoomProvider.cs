using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(RoomRepository roomRepository, SadieContext dbContext)
{
    public Task<List<Room>> GetRoomsForCategoryNameAsync(PlayerLogic player, string category)
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
        var searchQueryParts = searchQuery.Split(":");
        var filterName = searchQueryParts[0];
        var filterValue = searchQuery[(filterName.Length + 1)..];

        var query = dbContext
            .Rooms
            .Include(x => x.Owner)
            .Include(x => x.Settings)
            .Include(x => x.Tags);
        
        return filterName switch
        {
            "roomname" => await query
                .Where(x => x.Name.Contains(filterValue))
                .ToListAsync(),
            
            "owner" => await query
                .Where(x => x.Owner.Username.Contains(filterValue))
                .ToListAsync(),
            
            "tag" => await query
                .Where(x => x.Tags.Any(x => x.Name.Contains(filterValue)))
                .ToListAsync(),
            
            _ => []
        };
    }
}