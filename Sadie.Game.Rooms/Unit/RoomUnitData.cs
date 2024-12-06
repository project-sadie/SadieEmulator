using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Pathfinding;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Unit;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Enums.Unsorted;

namespace Sadie.Game.Rooms.Unit;

public class RoomUnitData(
    int id,
    IRoomLogic room,
    Point point,
    double pointZ,
    HDirection directionHead,
    HDirection direction,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomWiredService wiredService,
    IRoomPathFinderHelperService pathFinderHelperService) : IRoomUnitData
{
    public int Id { get; } = id;
    public HDirection DirectionHead { get; set; } = directionHead;
    public HDirection Direction { get; set; } = direction;
    public bool CanWalk { get; set; } = true;
    public Point Point { get; private set; } = point;
    public double PointZ { get; set; } = pointZ;
    public bool IsWalking { get; set; }
    protected bool NeedsPathCalculated { get; set; }
    public Point? NextPoint { get; set; }
    protected int StepsWalked { get; set; }
    protected Point PathGoal { get; set; }
    protected List<Point> PathPoints { get; set; } = [];
    public Dictionary<string, string> StatusMap { get; } = [];
    public bool NeedsStatusUpdate { get; set; }
    public List<Point> OverridePoints { get; set; } = [];

    public void RemoveStatuses(params string[] statuses)
    {
        foreach (var status in statuses)
        {
            StatusMap.Remove(status);
        }

        NeedsStatusUpdate = true;
    }

    private void ClearWalking(bool reachedGoal = true)
    {
        IsWalking = false;
        RemoveStatuses(RoomUserStatus.Move);
        CheckStatusForCurrentTile();

        if (reachedGoal && OnReachedGoal != null)
        {
            OnReachedGoal.Invoke();
            OnReachedGoal = null;
        }
    }
    
    protected Action? OnReachedGoal { get; set; }

    public void CheckStatusForCurrentTile()
    {
        if (IsWalking)
        {
            return;
        }
        
        var tileItems = tileMapHelperService.GetItemsForPosition(Point.X, Point.Y, room.FurnitureItems);

        if (tileItems.Count == 0)
        {
            PointZ = room.TileMap.ZMap[Point.X, Point.Y];
            RemoveStatuses(RoomUserStatus.Sit, RoomUserStatus.Lay);
        }

        var topItem = tileItems.MaxBy(item => item.PositionZ);

        if (topItem != null)
        {
            if (topItem.FurnitureItem.CanSit)
            {
                AddStatus(
                    RoomUserStatus.Sit, 
                    (topItem.FurnitureItem.StackHeight * 1.0D).ToString());
            
                Direction = topItem.Direction;
                DirectionHead = topItem.Direction;
            }
            else if (topItem.FurnitureItem.CanLay)
            {
                AddStatus(
                    RoomUserStatus.Lay, 
                    (topItem.FurnitureItem.StackHeight + 0.1).ToString());
            
                Direction = topItem.Direction;
                DirectionHead = topItem.Direction;
            }
        }
        
        var topItemSitOrLay = topItem?.FurnitureItem is { CanSit: false, CanLay: false };
        var zHeightNextStep = topItem?.FurnitureItem == null ?
            PointZ : 
            topItem.PositionZ + (topItemSitOrLay ? topItem.FurnitureItem.StackHeight : 0);
        
        PointZ = zHeightNextStep;
        NeedsStatusUpdate = true;
    }

    public void AddStatus(string key, string value)
    {
        StatusMap[key] = value;
        NeedsStatusUpdate = true;
    }

    private void CalculatePath()
    {
        PathPoints = pathFinderHelperService.BuildPathForWalk(room.TileMap, Point, PathGoal, room.Settings.WalkDiagonal, OverridePoints);

        if (PathPoints.Count > 1)
        {
            StepsWalked = 0;
            IsWalking = true;
            NeedsPathCalculated = false;
        }
        else
        {
            NeedsPathCalculated = true;
        }
    }
    
    public void WalkToPoint(Point point, Action? onReachedGoal = null)
    {
        if (room.TileMap.UsersAtPoint(point) &&
            !room.Settings.CanUsersOverlap)
        {
            return;
        }
        
        PathGoal = point;
        NeedsPathCalculated = true;
        OnReachedGoal = onReachedGoal;
    }

    protected async Task ProcessGenericChecksAsync()
    {
        if (NextPoint != null)
        {
            room.TileMap.UnitMap[Point].Remove(this);
            room.TileMap.AddUnitToMap(NextPoint.Value, this);

            await SetPositionAsync(NextPoint.Value);
            
            PointZ = NextZ;
            NextPoint = null;
        }
        
        if (NeedsPathCalculated)
        {
            CalculatePath();
        }

        if (IsWalking)
        {
            await ProcessMovementAsync();
        }
    }

    private async Task ProcessMovementAsync()
    {
        if (Point.X == PathGoal.X && Point.Y == PathGoal.Y || StepsWalked >= PathPoints.Count)
        {
            ClearWalking();
            return;
        }
        
        StepsWalked++;
        
        var nextStep = PathPoints[StepsWalked];
        var lastStep = PathPoints.Count == StepsWalked + 1;
        
        if (room.TileMap.Map[nextStep.Y, nextStep.X] == 0 && !OverridePoints.Contains(nextStep) || 
            (room.TileMap.Map[nextStep.Y, nextStep.X] == 2 && !lastStep) || 
            room.TileMap.UnitMap.GetValueOrDefault(nextStep, []).Count > 0)
        {
            NeedsPathCalculated = true;
            return;
        }

        var topItemNextStep = tileMapHelperService
            .GetItemsForPosition(nextStep.X, nextStep.Y, room.FurnitureItems)
            .MaxBy(x => x.PositionZ);

        var topItemSitOrLay = topItemNextStep?.FurnitureItem is { CanSit: false, CanLay: false };
        
        var zHeightNextStep = topItemNextStep?.FurnitureItem == null ?
            room.TileMap.ZMap[nextStep.Y, nextStep.X] : 
            topItemNextStep.PositionZ + (topItemSitOrLay ? topItemNextStep.FurnitureItem.StackHeight : 0);

        ClearStatuses();

        AddStatus(RoomUserStatus.Move, $"{nextStep.X},{nextStep.Y},{zHeightNextStep}");

        var newDirection = pathFinderHelperService.GetDirectionForNextStep(Point, nextStep);
                
        Direction = newDirection;
        DirectionHead = newDirection;
        NextZ = zHeightNextStep;
        NextPoint = nextStep;
    }

    public double NextZ { get; set; }
    public int HandItemId { get; set; }
    public DateTime HandItemSet { get; set; }
    
    public async Task SetPositionAsync(Point point)
    {
        Point = point;
    }

    private void ClearStatuses()
    {
        RemoveStatuses(
            RoomUserStatus.Sit,
            RoomUserStatus.Lay,
            RoomUserStatus.Move);
    }
}