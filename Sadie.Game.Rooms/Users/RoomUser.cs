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

    public void WalkToPoint(HPoint point)
    {
        PathGoal = point;
        NeedsPathCalculated = true;
    }

    public void LookAtPoint(HPoint point)
    {
        var direction = RoomHelpers.GetDirectionForNextStep(Point, point);

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
        if (NeedsPathCalculated)
        {
            PathPoints = RoomHelpers.BuildPathForWalk(room.TileMap, Point, PathGoal, room.Settings.WalkDiagonal);
            NeedsPathCalculated = false;

            if (PathPoints.Count > 0)
            {
                IsWalking = true;
            }
        }

        if (NextPoint != null)
        {
            Point = NextPoint;
            NextPoint = null;
        }

        if (IsWalking)
        {
            if (PathPoints.Count > 0)
            {
                NextPoint = PathPoints.Dequeue();
                
                RemoveStatuses(RoomUserStatus.Move);
                AddStatus(RoomUserStatus.Move, $"{NextPoint.X},{NextPoint.Y},{NextPoint.Z}");

                var direction = RoomHelpers.GetDirectionForNextStep(Point, NextPoint);

                Direction = direction;
                DirectionHead = direction;
            }
            else
            {
                RemoveStatuses(RoomUserStatus.Move);
                IsWalking = false;
            }
        }
        else if (StatusMap.ContainsKey(RoomUserStatus.Move))
        {
            RemoveStatuses(RoomUserStatus.Move);
        }

        await UpdateIdleStatusAsync();
    }

    private async Task UpdateIdleStatusAsync()
    {
        var shouldBeIdle = (DateTime.Now - LastAction) > IdleTime;

        if (shouldBeIdle && !IsIdle || !shouldBeIdle && IsIdle)
        {
            IsIdle = shouldBeIdle;
            await room!.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter(Id, IsIdle));
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

    public async ValueTask DisposeAsync()
    {
        
    }
}