using System.Drawing;
using Sadie.Database.Models.Constants;
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
    ServerRoomConstants constants,
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
        
        var currentTile = room.TileMap.GetTile(Point.X, Point.Y);
        currentTile?.Users.Remove(Id);
        RoomHelpers.UpdateTileMapForTile(currentTile!, room.TileMap);

        var nextTile = room.TileMap.GetTile(NextPoint.X, NextPoint.Y);
        nextTile?.Users.Add(Id, this);
        RoomHelpers.UpdateTileMapForTile(nextTile!, room.TileMap);

        Point = NextPoint;
    }

    public void WalkToPoint(Point point, bool useDiagonal)
    {
        StatusMap.Remove(RoomUserStatus.Lay);
        StatusMap.Remove(RoomUserStatus.Sit);
        StatusMap.Remove(RoomUserStatus.FlatCtrl);
        
        SetNextPosition();
        
        GoalSteps = RoomHelpers.BuildPathForWalk(room.TileMap, new Point(Point.X, Point.Y), point, useDiagonal);
        IsWalking = true;
    }

    public void StopWalking()
    {
        NextPoint = null;
        IsWalking = false;

        StatusMap.Remove(RoomUserStatus.Move);
        
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
        StatusMap[RoomUserStatus.FlatCtrl] = ((int) controllerLevel).ToString();
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
        
        var currentTile = room.TileMap.GetTile(Point.X, Point.Y);
        var tileItems = currentTile?.Items;

        if (tileItems == null || tileItems.Count == 0)
        {
            StatusMap.Remove(RoomUserStatus.Sit);
            StatusMap.Remove(RoomUserStatus.Lay);
            return;
        }

        var topItem = tileItems.MaxBy(item => item.PositionZ);

        if (topItem == null)
        {
            StatusMap.Remove(RoomUserStatus.Sit);
            StatusMap.Remove(RoomUserStatus.Lay);
            return;
        }
        
        if (topItem.FurnitureItem.CanSit)
        {
            StatusMap[RoomUserStatus.Sit] = (topItem.PositionZ + topItem.FurnitureItem.StackHeight) + "";
            
            Direction = topItem.Direction;
            DirectionHead = topItem.Direction;
        }
        else if (topItem.FurnitureItem.CanLay)
        {
            StatusMap[RoomUserStatus.Lay] = (topItem.PositionZ + topItem.FurnitureItem.StackHeight) + "";
            
            Direction = topItem.Direction;
            DirectionHead = topItem.Direction;
        }
        else
        {
            StatusMap.Remove(RoomUserStatus.Sit);
            StatusMap.Remove(RoomUserStatus.Lay);
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