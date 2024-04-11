using System.Drawing;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUser(
    RoomLogic room,
    INetworkObject networkObject,
    int id,
    HPoint point,
    HDirection directionHead,
    HDirection direction,
    PlayerLogic player,
    RoomConstants constants,
    RoomControllerLevel controllerLevel)
    : RoomUserData(point, directionHead, direction, player, TimeSpan.FromSeconds(constants.SecondsTillUserIdle)),
        IRoomUser
{
    public int Id { get; } = id;
    public RoomControllerLevel ControllerLevel { get; set; } = controllerLevel;
    public INetworkObject NetworkObject { get; } = networkObject;

    private void SetNextPosition()
    {
        if (NextPoint == null)
        {
            return;
        }
        
        var currentTile = room.TileMap.FindTile(Point.X, Point.Y);
        currentTile?.Users.Remove(Id);
        RoomHelpers.UpdateTileMapForTile(currentTile!, room.TileMap);

        var nextTile = room.TileMap.FindTile(NextPoint.X, NextPoint.Y);
        nextTile?.Users.Add(Id, this);
        RoomHelpers.UpdateTileMapForTile(nextTile!, room.TileMap);

        Point = NextPoint;
    }

    public void WalkToPoint(Point point, bool useDiagonal)
    {
        RemoveStatuses(
            RoomUserStatus.Lay,
            RoomUserStatus.Sit,
            RoomUserStatus.FlatCtrl);
        
        SetNextPosition();
        
        GoalSteps = RoomHelpers.BuildPathForWalk(room.TileMap, new Point(Point.X, Point.Y), point, useDiagonal);
        IsWalking = true;
    }

    public void StopWalking()
    {
        NextPoint = null;
        IsWalking = false;

        RemoveStatuses(RoomUserStatus.Move);
        ApplyFlatCtrlStatus();
        CheckStatusForCurrentTile();
    }

    public void LookAtPoint(Point point)
    {
        var direction = RoomHelpers.GetDirectionForNextStep(Point.ToPoint(), point);

        Direction = direction;
        DirectionHead = direction;
        LastAction = DateTime.Now;
    }

    public void ApplyFlatCtrlStatus()
    {
        AddStatus(RoomUserStatus.FlatCtrl, ((int)controllerLevel).ToString());
    }

    public void AddStatus(string key, string value)
    {
        StatusMap[key] = value;
        NeedsStatusUpdate = true;
    }

    public void RemoveStatuses(params string[] statuses)
    {
        foreach (var status in statuses)
        {
            StatusMap.Remove(status);
        }

        NeedsStatusUpdate = true;
    }

    public async Task RunPeriodicCheckAsync()
    {
        if (IsWalking)
        {
            await ProcessMovementAsync();
        }

        await UpdateIdleStatusAsync();
    }

    private async Task UpdateIdleStatusAsync()
    {
        var shouldBeIdle = (DateTime.Now - LastAction) > IdleTime;

        if (shouldBeIdle && !IsIdle || !shouldBeIdle && IsIdle)
        {
            IsIdle = shouldBeIdle;
            await room!.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter(Id, IsIdle).GetAllBytes());
        }
    }

    public void UpdateLastAction()
    {
        LastAction = DateTime.Now;
    }

    public void CheckStatusForCurrentTile()
    {
        if (IsWalking)
        {
            return;
        }
        
        var currentTile = room.TileMap.FindTile(Point.X, Point.Y);
        var tileItems = currentTile?.Items;

        if (tileItems == null || tileItems.Count == 0)
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

    public bool HasRights()
    {
        return room.OwnerId == Id || 
               room.PlayerRights.FirstOrDefault(x => x.PlayerId == Id) != null;
    }

    private async Task ProcessMovementAsync() // 2bMoved
    {
        SetNextPosition();

        if (GoalSteps.Count > 0)
        {
            var next = GoalSteps.Dequeue();
            
            LookAtPoint(next.ToPoint());
            
            StatusMap[RoomUserStatus.Move] = $"{next.X},{next.Y},{Math.Round(next.Z * 100.0) / 100.0}";
            NeedsStatusUpdate = true;
            
            NextPoint = next;
        }
        else
        {
            StopWalking();
        }
    }

    public async ValueTask DisposeAsync()
    {
        
    }
}