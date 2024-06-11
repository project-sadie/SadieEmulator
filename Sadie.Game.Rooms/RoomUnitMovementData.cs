using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Game.Rooms;

public class RoomUnitMovementData(IRoomLogic room) : IRoomUnitMovementData
{
    public HDirection DirectionHead { get; set; }
    public HDirection Direction { get; set; }
    public Point Point { get; set;  }
    public double PointZ { get; init;  }
    public bool IsWalking { get; set; }
    protected bool NeedsPathCalculated { get; set; }
    protected Point? NextPoint { get; set; }
    protected int StepsWalked { get; set; }
    protected Point PathGoal { get; set; }
    protected List<Point> PathPoints { get; set; } = [];
    public Dictionary<string, string> StatusMap { get; } = [];
    public bool NeedsStatusUpdate { get; set; }

    public void RemoveStatuses(params string[] statuses)
    {
        foreach (var status in statuses)
        {
            StatusMap.Remove(status);
        }

        NeedsStatusUpdate = true;
    }

    private void ClearWalking()
    {
        IsWalking = false;
        RemoveStatuses(RoomUserStatus.Move);
        CheckStatusForCurrentTile();

        if (OnReachedGoal != null)
        {
            OnReachedGoal.Invoke();
            OnReachedGoal = null;
        }
    }
    
    protected Action? OnReachedGoal { get; set; } = null;

    public void CheckStatusForCurrentTile()
    {
        if (IsWalking)
        {
            return;
        }
        
        var tileItems = RoomTileMapHelpers.GetItemsForPosition(Point.X, Point.Y, room.FurnitureItems);

        if (tileItems.Count == 0)
        {
            RemoveStatuses(RoomUserStatus.Sit, RoomUserStatus.Lay);
            return;
        }

        var topItem = tileItems.MaxBy(item => item.PositionZ);

        if (topItem == null)
        {
            RemoveStatuses(RoomUserStatus.Sit, RoomUserStatus.Lay);
            return;
        }
        
        if (topItem.FurnitureItem.CanSit)
        {
            AddStatus(
                RoomUserStatus.Sit, 
                (topItem.PositionZ + topItem.FurnitureItem.StackHeight).ToString());
            
            Direction = topItem.Direction;
            DirectionHead = topItem.Direction;
        }
        else if (topItem.FurnitureItem.CanLay)
        {
            AddStatus(
                RoomUserStatus.Lay, 
                (topItem.PositionZ + topItem.FurnitureItem.StackHeight).ToString());
            
            Direction = topItem.Direction;
            DirectionHead = topItem.Direction;
        }
        else
        {
            RemoveStatuses(
                RoomUserStatus.Sit,
                RoomUserStatus.Lay);
        }
    }

    public void AddStatus(string key, string value)
    {
        StatusMap[key] = value;
        NeedsStatusUpdate = true;
    }

    protected void CalculatePath(bool diagonal)
    {
        PathPoints = RoomPathFinderHelpers.BuildPathForWalk(room.TileMap, Point, PathGoal, diagonal);

        if (PathPoints.Count > 1)
        {
            StepsWalked = 0;
            IsWalking = true;
            NeedsPathCalculated = false;
        }
        else
        {
            NeedsPathCalculated = false;
                
            if (PathPoints.Count > 0)
            {
                PathPoints.Clear();
            }
        }
    }

    protected void ProcessMovement()
    {
        if (Point.X == PathGoal.X && Point.Y == PathGoal.Y || StepsWalked >= PathPoints.Count)
        {
            ClearWalking();
            return;
        }
        
        StepsWalked++;
        
        var nextStep = PathPoints[StepsWalked];
        var lastStep = PathPoints.Count == StepsWalked + 1;
        
        if (room.TileMap.Map[nextStep.Y, nextStep.X] == 0 || 
            (room.TileMap.Map[nextStep.Y, nextStep.X] == 2 && !lastStep) || 
            room.TileMap.UserMap.GetValueOrDefault(nextStep, [])!.Count > 0)
        {
            NeedsPathCalculated = true;
            return;
        }
        
        var itemsAtNextStep = RoomTileMapHelpers.GetItemsForPosition(nextStep.X, nextStep.Y, room.FurnitureItems);
        var nextZ = itemsAtNextStep.Count < 1 ? 0 : itemsAtNextStep.MaxBy(x => x.PositionZ)?.PositionZ;

        ClearStatuses();

        AddStatus(RoomUserStatus.Move, $"{nextStep.X},{nextStep.Y},{nextZ}");

        var newDirection = RoomPathFinderHelpers.GetDirectionForNextStep(Point, nextStep);
                
        Direction = newDirection;
        DirectionHead = newDirection;
                
        NextPoint = nextStep;
    }
    
    public void WalkToPoint(Point point, Action? onReachedGoal)
    {
        PathGoal = point;
        NeedsPathCalculated = true;
        OnReachedGoal = onReachedGoal;
    }
    
    private void ClearStatuses()
    {
        RemoveStatuses(
            RoomUserStatus.Sit,
            RoomUserStatus.Lay,
            RoomUserStatus.Move);
    }
}