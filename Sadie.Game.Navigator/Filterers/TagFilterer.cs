using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Navigator.Filterers;

public class TagFilterer : INavigatorSearchFilterer
{
    public string Name => "tag";
    
    public IQueryable<Room> ApplyFilter(
        IQueryable<Room> query, 
        string value)
    {
        return  query
            .Where(r => r.Tags.Any(t => t.Name.Contains(value)));
    }
}


