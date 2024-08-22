using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Navigator.Filterers;

public class RoomNameFilterer : INavigatorSearchFilterer
{
    public string Name => "roomname";
    public IQueryable<Room> ApplyFilter(IQueryable<Room> query, string value)
    {
        return query.Where(x => x.Name.Contains(value));
    }
}