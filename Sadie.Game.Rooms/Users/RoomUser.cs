using System.Drawing;
using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Networking;
using Sadie.Shared.Extensions;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUser(
    IRoomData room,
    INetworkObject networkObject,
    int id,
    HPoint point,
    HDirection directionHead,
    HDirection direction,
    AvatarData avatarData,
    RoomConstants constants,
    RoomControllerLevel controllerLevel)
    : RoomUserData(point, directionHead, direction, avatarData, TimeSpan.FromSeconds(constants.SecondsTillUserIdle)),
        IRoomUser
{
    public int Id { get; } = id;
    public RoomControllerLevel ControllerLevel { get; } = controllerLevel;
    public INetworkObject NetworkObject { get; } = networkObject;

    private void SetNextPosition()
    {
        Point = NextPoint ?? Point;
    }
    
    public void WalkToPoint(Point point, bool useDiagonal)
    {
        StatusMap.Clear();
        
        SetNextPosition();
        
        GoalSteps = RoomHelpers.BuildPathForWalk(room.Layout, new Point(Point.X, Point.Y), point, useDiagonal);
        IsWalking = true;
    }

    public void StopWalking()
    {
        NextPoint = null;
        IsWalking = false;

        StatusMap.Remove(RoomUserStatus.Move);
    }

    public void LookAtPoint(Point point)
    {
        var direction = RoomHelpers.GetDirectionForNextStep(Point.ToPoint(), point);

        Direction = direction;
        DirectionHead = direction;
    }

    public async Task RunPeriodicCheckAsync()
    {
        if (IsWalking)
        {
            await ProcessMovementAsync();
        }
    }

    private void CheckStatusForCurrentTile()
    {
        var currentTile = room.Layout.FindTile(Point.X, Point.Y);
        
        var seat = currentTile?
            .Items
            .OrderByDescending(item => item.Position.Z)
            .FirstOrDefault(x => x.FurnitureItem.CanSit);

        if (seat == null)
        {
            return;
        }
        
        Direction = seat.Direction;
        DirectionHead = seat.Direction;
        
        StatusMap[RoomUserStatus.Sit] = seat.Position.Z + "";
    }

    public bool HasRights()
    {
        return room.OwnerId == Id || room.PlayersWithRights.Contains(Id);
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
            CheckStatusForCurrentTile();
        }
    }

    public async Task OnTalkAsync(RoomChatMessage message)
    {
        room.ChatMessages.Add(message);
    }

    public async ValueTask DisposeAsync()
    {
        
    }
}