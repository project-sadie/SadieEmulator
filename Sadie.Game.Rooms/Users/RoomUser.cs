using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Enums.Game.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Packets.Writers;
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
    RoomControllerLevel controllerLevel)
    : RoomUserData(room, point, pointZ, directionHead, direction, player, TimeSpan.FromSeconds(constants.SecondsTillUserIdle)),
        IRoomUser
{
    public int Id { get; } = id;
    public IRoomLogic Room { get; } = room;
    public RoomControllerLevel ControllerLevel { get; set; } = controllerLevel;
    public INetworkObject NetworkObject { get; } = networkObject;

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

    public async Task RunPeriodicCheckAsync()
    {
        if (NextPoint != null)
        {
            room.TileMap.UserMap[Point].Remove(this);
            room.TileMap.AddUserToMap(NextPoint.Value, this);
            
            Point = NextPoint.Value;
            NextPoint = null;
        }

        if (NeedsPathCalculated)
        {
            CalculatePath(room.Settings.WalkDiagonal);
        }

        if (IsWalking)
        {
            ProcessMovement();
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