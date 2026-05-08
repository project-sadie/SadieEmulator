using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo;

namespace Sadie.Game.Rooms.PathFinding.ToGo.Collections.PathFinder;

public readonly struct PathFinderNode(IPosition p, int g, int h, IPosition parent)
{
    public IPosition Position { get; } = p;
    public int G { get; } = g;
    public int H { get; } = h;
    public int F { get; } = g + h;
    public IPosition ParentNodePosition { get; } = parent;
}