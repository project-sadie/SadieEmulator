using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Navigator;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.Db;
using Sadie.Db.Models.Rooms;
using Sadie.Game.Navigator.Filterers;

namespace Sadie.Game.Navigator;

public class NavigatorRoomProvider(
    IRoomRepository roomRepository, 
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IEnumerable<INavigatorSearchFilterer> filterers,
    IMapper mapper) : INavigatorRoomProvider
{
    public Task<List<RoomDto>> GetRoomsForCategoryNameAsync(IPlayerLogic player, string category)
    {
        return Task.FromResult(category switch
        {
            "popular" => roomRepository.GetPopularRooms(50),
            "my_rooms" => player.Player.Rooms.ToList(),
            _ => []
        });
    }

    public async Task<List<RoomDto>> GetRoomsForSearchQueryAsync(string searchQuery)
    {
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var query = dbContext
            .Rooms
            .AsQueryable();
        
        if (searchQuery.Contains(':'))
        {
            query = ApplyFilter(query, searchQuery.Split([':'], 2));
        }
        else
        {
            query = query.Where(x =>
                x.Name.Contains(searchQuery) ||
                x.Description.Contains(searchQuery) ||
                x.Tags.Any(t => t.Name.Contains(searchQuery)) ||
                x.Owner!.Username.Contains(searchQuery));
        }

        var rooms = await query
            .Include(x => x.Settings)
            .Include(x => x.Layout)
            .Include(x => x.FurnitureItems)
            .Include(x => x.Owner)
            .Include(x => x.PaintSettings)
            .Include(x => x.ChatSettings)
            .Include(x => x.PlayerLikes)
            .Include(x => x.Tags)
            .Include(x => x.Group)
            .Include(x => x.DimmerSettings)
            .ToListAsync();
        
        return mapper.Map<List<RoomDto>>(rooms);
    }

    private IQueryable<Room> ApplyFilter(IQueryable<Room> query, IReadOnlyList<string> filterData)
    {
        var filterer = filterers.FirstOrDefault(x => x.Name == filterData[0]);

        return filterer != null ? 
            filterer.Apply(query, filterData[1]) : 
            query;
    }
}