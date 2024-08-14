using System.Drawing;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Unsorted;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Packets.Writers.Users.HandItems;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Shared.Unsorted;
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
    : RoomUserData(id, room, point, pointZ, directionHead, direction, player, TimeSpan.FromSeconds(constants.SecondsTillUserIdle)),
        IRoomUser
{
    public int Id { get; } = id;
    public IRoomLogic Room { get; } = room;
    public RoomControllerLevel ControllerLevel { get; set; } = controllerLevel;
    public INetworkObject NetworkObject { get; } = networkObject;

    public void LookAtPoint(Point point)
    {
        var direction = RoomPathFinderHelpers.GetDirectionForNextStep(Point, point);

        if (!StatusMap.ContainsKey(RoomUserStatus.Sit))
        {
            Direction = direction;
        }

        if (Math.Abs(direction - Direction) < 2)
        {
            DirectionHead = direction;
        }

        NeedsStatusUpdate = true;
    }

    public void ApplyFlatCtrlStatus()
    {
        AddStatus(RoomUserStatus.FlatCtrl, ((int)controllerLevel).ToString());
    }

    public async Task RunPeriodicCheckAsync()
    {
        if (HandItemId != 0 && (DateTime.Now - HandItemSet).TotalSeconds >= 30)
        {
            HandItemId = 0;
        
            await room.UserRepository.BroadcastDataAsync(new RoomUserHandItemWriter
            {
                UserId = Id,
                ItemId = 0
            });
        }
        
        if (NextPoint != null)
        {
            room.TileMap.UserMap[Point].Remove(this);
            room.TileMap.AddUserToMap(NextPoint.Value, this);
            
            Point = NextPoint.Value;
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
            
            await room.UserRepository.BroadcastDataAsync(writer);
        }
    }

    public bool HasRights()
    {
        return ControllerLevel is 
            RoomControllerLevel.Owner or 
            RoomControllerLevel.Rights;
    }

    public async ValueTask DisposeAsync()
    {
        room.TileMap.UserMap[Point].Remove(this);
    }
}