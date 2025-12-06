using System.Drawing;
using Sadie.API.Interfaces.Game.Rooms.Pathfinding;
using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo;
using Sadie.API.Interfaces.Game.Rooms.Pathfinding.ToGo.Heuristics;
using Sadie.Game.Rooms.PathFinding.ToGo;
using Sadie.Game.Rooms.PathFinding.ToGo.Collections.PathFinder;
using Sadie.Game.Rooms.PathFinding.ToGo.Heuristics;
using Sadie.Game.Rooms.PathFinding.ToGo.Options;

namespace Sadie.Game.Rooms.PathFinding;

public class RoomPathFinder : IRoomPathFinder
{
    private const int Closed = 0;
    private const int StepCost = 1;

    private readonly PathFinderOptions _opts;
    private readonly ICalculateHeuristic _heuristic;
    private readonly PathFinderGraph _graph;

    private readonly IPosition[] _backtrackBuf;
    private readonly Point[] _pointBuf;

    public RoomPathFinder(int height, int width, PathFinderOptions? opts = null)
    {
        _opts = opts ?? new PathFinderOptions();
        _heuristic = HeuristicFactory.Create(_opts.HeuristicFormula);

        _graph = new PathFinderGraph(height, width, _opts.UseDiagonals);

        _backtrackBuf = new IPosition[height * width];
        _pointBuf = new Point[height * width];
    }

    public IEnumerable<Point> FindPath(Point start, Point end, IWorldGrid world)
    {
        var result = FindPath(
            new Position(start.Y, start.X),
            new Position(end.Y, end.X),
            world,
            out var count);

        for (var i = 0; i < count; i++)
        {
            _pointBuf[i] = new Point(result[i].Column, result[i].Row);
        }

        return _pointBuf.Take(count);
    }

    public IEnumerable<IPosition> FindPath(IPosition start, IPosition end, IWorldGrid world)
    {
        var pos = FindPath(
            new Position(start.Row, start.Column),
            new Position(end.Row, end.Column),
            world,
            out var count);

        return pos.Take(count);
    }

    private IPosition[] FindPath(IPosition start, IPosition end, IWorldGrid world, out int outCount)
    {
        _graph.Reset();

        var startNode = new PathFinderNode(start, 0, 0, start);
        _graph.OpenNode(startNode);

        var visited = 0;

        while (_graph.HasOpenNodes)
        {
            var q = _graph.GetOpenNodeWithSmallestF();

            if (q.Position.Row == end.Row && q.Position.Column == end.Column)
            {
                outCount = Backtrack(q);
                return _backtrackBuf;
            }

            if (visited++ > _opts.SearchLimit)
            {
                outCount = 0;
                return _backtrackBuf;
            }

            foreach (var s in _graph.GetSuccessors(q))
            {
                if (world[s.Position] == Closed)
                {
                    continue;
                }

                var g = q.G + StepCost;

                if (_opts.PunishChangeDirection)
                {
                    g += CalculateModifier(q, s, end);
                }

                var n = new PathFinderNode(
                    s.Position,
                    g,
                    _heuristic.Calculate(s.Position, end),
                    q.Position);

                if (!_graph.WasVisited(s.Position) || n.F < s.F)
                {
                    _graph.OpenNode(n);
                }
            }
        }

        outCount = 0;
        return _backtrackBuf;
    }

    private int Backtrack(PathFinderNode endNode)
    {
        var count = 0;
        var current = endNode;

        var guard = 0;

        while (!current.Position.Equals(current.ParentNodePosition))
        {
            if (guard++ > 50000)
            {
                return 0;
            }

            _backtrackBuf[count++] = current.Position;
            current = _graph.GetParent(current);
        }

        _backtrackBuf[count++] = current.Position;
        Array.Reverse(_backtrackBuf, 0, count);
        
        return count;
    }

    private static int CalculateModifier(PathFinderNode q, PathFinderNode s, IPosition end)
    {
        if (q.Position == q.ParentNodePosition)
        {
            return 0;
        }

        var p = Math.Abs(s.Position.Row - end.Row) +
                Math.Abs(s.Position.Column - end.Column);

        var v = s.Position.Row != q.Position.Row;
        if (v)
        {
            var pv = q.Position.Row == q.ParentNodePosition.Row;
            if (pv)
            {
                return p;
            }
        }

        var h = s.Position.Column != q.Position.Column;
        
        if (h)
        {
            var ph = q.Position.Column == q.ParentNodePosition.Column;
            if (ph)
            {
                return p;
            }
        }

        return 0;
    }
}
