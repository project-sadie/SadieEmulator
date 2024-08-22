using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Navigator.Filterers;

public class OwnerFilterer : INavigatorSearchFilterer
{
    public string Name => "owner";
    
    public IQueryable<Room> ApplyFilter(IQueryable<Room> query, string value)
    {
        return query
            .Where(x => x.Owner.Username.Contains(value));
    }
}