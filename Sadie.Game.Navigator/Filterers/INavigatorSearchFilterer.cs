using Sadie.Database.Models.Rooms;

namespace Sadie.Game.Navigator.Filterers;

public interface INavigatorSearchFilterer
{
    public string Name { get; }
    IQueryable<Room> ApplyFilter(IQueryable<Room> query, string value);
}