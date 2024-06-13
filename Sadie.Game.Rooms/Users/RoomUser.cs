using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.PathFinding;
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
    RoomControllerLevel controllerLevel,
    RoomFurnitureItemInteractorRepository interactorRepository)
    : RoomUserData(id, room, point, pointZ, directionHead, direction, player, TimeSpan.FromSeconds(constants.SecondsTillUserIdle), interactorRepository),
        IRoomUser
{
    public int Id { get; } = id;
    public IRoomLogic Room { get; } = room;
    public RoomControllerLevel ControllerLevel { get; set; } = controllerLevel;
    public INetworkObject NetworkObject { get; } = networkObject;

    private void ClearStatuses()
    {
        RemoveStatuses(
            RoomUserStatus.Sit,
            RoomUserStatus.Lay,
            RoomUserStatus.Move);
    }

    public void LookAtPoint(Point point)
    {
        var direction = RoomPathFinderHelpers.GetDirectionForNextStep(Point, point);

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
        PathPoints = RoomPathFinderHelpers.BuildPathForWalk(room.TileMap, Point, PathGoal, room.Settings.WalkDiagonal, OverridePoints);

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

    public async Task RunPeriodicCheckAsync()
    {
        if (NextPoint != null)
        {
            room.TileMap.UserMap[Point].Remove(this);
            room.TileMap.AddUserToMap(NextPoint.Value, this);
            
            Point = NextPoint.Value;
            NextPoint = null;
            
            foreach (var item in RoomTileMapHelpers.GetItemsForPosition(Point.X, Point.Y, room.FurnitureItems))
            {
                var interactor = interactorRepository.GetInteractorForType(item.FurnitureItem.InteractionType);
            
                if (interactor != null)
                {
                    await interactor.OnStepOnAsync(room, item, Unit);
                }
            }
        }

        if (NeedsPathCalculated)
        {
            CalculatePath();
        }

        if (IsWalking)
        {
            await ProcessMovementAsync();
        }

        await UpdateIdleStatusAsync();
    }
    
    private async Task UpdateIdleStatusAsync()
    {
        var shouldBeIdle = DateTime.Now - LastAction > IdleTime;

        if (shouldBeIdle && !IsIdle || !shouldBeIdle && IsIdle)
        {
            IsIdle = shouldBeIdle;

            var writer = new RoomUserIdleWriter
            {
                UserId = Id,
                IsIdle = IsIdle
            };
            
            await room!.UserRepository.BroadcastDataAsync(writer);
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

    public bool HasRights()
    {
        return room.OwnerId == Id || 
               room.PlayerRights.FirstOrDefault(x => x.PlayerId == Id) != null;
    }

    public async ValueTask DisposeAsync()
    {
        room.TileMap.UserMap[Point].Remove(this);
    }
}