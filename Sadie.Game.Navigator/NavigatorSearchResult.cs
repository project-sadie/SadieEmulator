using Sadie.Game.Rooms;

namespace Sadie.Game.Navigator;

public class NavigatorSearchResult
{
    public NavigatorSearchResult(int order, string unknown1, string unknown2, List<IRoom> rooms, NavigatorSearchAction action, bool isCollapsed)
    {
        Order = order;
        Unknown1 = unknown1;
        Unknown2 = unknown2;
        Rooms = rooms;
        Action = action;
        IsCollapsed = isCollapsed;
    }

    public int Order { get; }
    public string Unknown1 { get; }
    public string Unknown2 { get; }
    public List<IRoom> Rooms { get; }
    public NavigatorSearchAction Action { get; }
    public bool IsCollapsed { get; }
}