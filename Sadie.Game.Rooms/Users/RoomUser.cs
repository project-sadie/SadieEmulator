using System.Drawing;
using Sadie.Database.Models.Constants;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Enums;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Game.Rooms;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Rooms.Users;

public class RoomUser(
    RoomLogic room,
    INetworkObject networkObject,
    int id,
    Point point,
    double pointZ,
    HDirection directionHead,
    HDirection direction,
    PlayerLogic player,
    ServerRoomConstants constants,
    RoomControllerLevel controllerLevel)
    : RoomUserData(point, pointZ, directionHead, direction, player, TimeSpan.FromSeconds(constants.SecondsTillUserIdle)),
        IRoomUser
{
    public int Id { get; } = id;
    public RoomLogic Room { get; } = room;
    public RoomControllerLevel ControllerLevel { get; set; } = controllerLevel;
    public INetworkObject NetworkObject { get; } = networkObject;

    private void ClearStatuses()
    {
        RemoveStatuses(
            RoomUserStatus.Sit,
            RoomUserStatus.Lay,
            RoomUserStatus.Move);
    }
    
    public void WalkToPoint(Point point)
    {
        PathGoal = point;
        NeedsPathCalculated = true;
    }

    public void LookAtPoint(Point point)
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

    private void CalculatePath()
    {
        PathPoints = RoomHelpers.BuildPathForWalk(room.TileMap, Point, PathGoal, room.Settings.WalkDiagonal);

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

    public async Task RunPeriodicCheckAsync()
    {
        if (NextPoint != null)
        {
            room.TileMap.RemoveUserFromMap(Point, this);
            room.TileMap.AddUserToMap(NextPoint.Value, this);
            
            Point = NextPoint.Value;
            NextPoint = null;
        }

        if (NeedsPathCalculated)
        {
            CalculatePath();
        }

        if (IsWalking)
        {
            ProcessMovement();
        }

        await UpdateIdleStatusAsync();
    }

    private void ClearWalking()
    {
        IsWalking = false;
        RemoveStatuses(RoomUserStatus.Move);
        CheckStatusForCurrentTile();
    }
    
    private void ProcessMovement()
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
            room.TileMap.GetMappedUsers(nextStep).Count > 0)
        {
            NeedsPathCalculated = true;
            return;
        }
        
        var itemsAtNextStep = RoomHelpers.GetItemsForPosition(nextStep.X, nextStep.Y, room.FurnitureItems);
        var nextZ = itemsAtNextStep.Count < 1 ? 0 : itemsAtNextStep.MaxBy(x => x.PositionZ)?.PositionZ;

        ClearStatuses();

        AddStatus(RoomUserStatus.Move, $"{nextStep.X},{nextStep.Y},{nextZ}");

        var newDirection = RoomHelpers.GetDirectionForNextStep(Point, nextStep);
                
        Direction = newDirection;
        DirectionHead = newDirection;
                
        NextPoint = nextStep;
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
        
        var tileItems = RoomHelpers.GetItemsForPosition(Point.X, Point.Y, room.FurnitureItems);

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

    public bool HasRights()
    {
        return room.OwnerId == Id || 
               room.PlayerRights.FirstOrDefault(x => x.PlayerId == Id) != null;
    }

    public async ValueTask DisposeAsync()
    {
        room.TileMap.RemoveUserFromMap(Point, this);
    }
}