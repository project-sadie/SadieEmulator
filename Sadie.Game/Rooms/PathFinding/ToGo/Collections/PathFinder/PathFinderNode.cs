using System.Runtime.InteropServices;

namespace Sadie.Game.Rooms.PathFinding.ToGo.Collections.PathFinder;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal readonly struct PathFinderNode(Position position, int g, int h, Position parentNodePosition)
{
    /// <summary>
    /// The position of the node
    /// </summary>
    public Position Position { get; } = position;

    /// <summary>
    /// Distance from home
    /// </summary>
    public int G { get; } = g;

    /// <summary>
    /// Heuristic
    /// </summary>
    public int H { get; } = h;

    /// <summary>
    /// This nodes parent
    /// </summary>
    public Position ParentNodePosition { get; } = parentNodePosition;

    /// <summary>
    /// Gone + Heuristic (H)
    /// </summary>
    public int F { get; } = g + h;

    /// <summary>
    /// If the node has been considered yet
    /// </summary>
    public bool HasBeenVisited => F > 0;
}