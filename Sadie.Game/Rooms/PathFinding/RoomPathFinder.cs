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
        var count = InternalFindPath(new Position(start.Y, start.X), new Position(end.Y, end.X), world);
        
        for (var i = 0; i < count; i++)
        {
            var p = _backtrackBuf[i];
            _pointBuf[i] = new Point(p.Column, p.Row);
        }
        
        return CreatePointEnumerable(count);
    }

    public IEnumerable<IPosition> FindPath(IPosition start, IPosition end, IWorldGrid world)
    {
        var count = InternalFindPath(new Position(start.Row, start.Column), new Position(end.Row, end.Column), world);
        return CreatePositionEnumerable(count);
    }

    private int InternalFindPath(Position start, Position end, IWorldGrid world)
    {
        _graph.Reset();
        
        var startNode = new PathFinderNode(start, 0, 0, start);
        _graph.OpenNode(startNode);
        
        var visited = 0;
        
        while (_graph.HasOpenNodes)
        {
            var q = _graph.GetOpenNodeWithSmallestF();
            
            if (q.Position.Equals(end))
            {
                return Backtrack(q);
            }
            
            if (visited++ > _opts.SearchLimit)
            {
                return 0;
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
                
                var n = new PathFinderNode(s.Position, g, _heuristic.Calculate(s.Position, end), q.Position);
                
                if (!_graph.WasVisited(s.Position) || n.F < s.F)
                {
                    _graph.OpenNode(n);
                }
            }
        }
        
        return 0;
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
        var left = 0;
        var right = count - 1;
        
        while (left < right)
        {
            var tmp = _backtrackBuf[left];
            
            _backtrackBuf[left] = _backtrackBuf[right];
            _backtrackBuf[right] = tmp;
            
            left++;
            right--;
        }
        
        for (var i = count; i < _backtrackBuf.Length; i++)
        {
            _backtrackBuf[i] = null;
        }
        
        return count;
    }

    private static int CalculateModifier(PathFinderNode q, PathFinderNode s, IPosition end)
    {
        if (q.Position == q.ParentNodePosition)
        {
            return 0;
        }
        
        var p = Math.Abs(s.Position.Row - end.Row) + Math.Abs(s.Position.Column - end.Column);
        
        if (s.Position.Row != q.Position.Row)
        {
            if (q.Position.Row == q.ParentNodePosition.Row)
            {
                return p;
            }
        }
        
        if (s.Position.Column != q.Position.Column)
        {
            if (q.Position.Column == q.ParentNodePosition.Column)
            {
                return p;
            }
        }
        
        return 0;
    }

    private IEnumerable<Point> CreatePointEnumerable(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return _pointBuf[i];
        }
    }

    private IEnumerable<IPosition> CreatePositionEnumerable(int count)
    {
        for (var i = 0; i < count; i++)
        {
            yield return _backtrackBuf[i];
        }
    }
}
