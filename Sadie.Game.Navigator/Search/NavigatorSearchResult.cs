using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator.Search;

public class NavigatorSearchResult
{
    public NavigatorSearchResult(int order, string unknown1, string unknown2, List<IRoom> rooms, NavigatorSearchButton button, bool isCollapsed)
    {
        Order = order;
        Unknown1 = unknown1;
        Unknown2 = unknown2;
        Rooms = rooms;
        Button = button;
        IsCollapsed = isCollapsed;
    }

    public int Order { get; }
    public string Unknown1 { get; }
    public string Unknown2 { get; }
    public List<IRoom> Rooms { get; }
    public NavigatorSearchButton Button { get; }
    public bool IsCollapsed { get; }
}