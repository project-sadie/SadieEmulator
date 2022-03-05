using System.Drawing;
using Microsoft.Extensions.Logging;
using Sadie.Shared.Networking;
using Sadie.Shared;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUser : RoomUserData, IRoomUser
{
    private readonly ILogger<RoomUser> _logger;
    private readonly Room _room;
    private readonly IRoomUserRepository _roomUserRepository;
    public INetworkObject NetworkObject { get; }

    public RoomUser(
        ILogger<RoomUser> logger,
        Room room,
        IRoomUserRepository roomUserRepository,
        INetworkObject networkObject, 
        long id, 
        HPoint point, 
        HDirection directionHead, 
        HDirection direction, 
        string username, 
        string motto, 
        string figureCode, 
        string gender, 
        long achievementScore) : 
        base(id, point, directionHead, direction, username, motto, figureCode, gender, achievementScore)
    {
        _logger = logger;
        _room = room;
        _roomUserRepository = roomUserRepository;
        NetworkObject = networkObject;
    }

    private void SetNextPosition()
    {
        Point = NextPoint ?? Point;
    }
    
    public void WalkToPoint(Point point, bool useDiagonal) // 2bMoved
    {
        SetNextPosition();
        
        GoalSteps = RoomHelpers.BuildPathForWalk(_room.Layout, new Point(Point.X, Point.Y), point, useDiagonal);
        IsWalking = true;
    }

    public async Task RunPeriodicCheckAsync()
    {
        if (IsWalking)
        {
            await ProcessMovementAsync();
        }
    }

    private async Task ProcessMovementAsync() // 2bMoved
    {
        SetNextPosition();

        if (GoalSteps.Count > 0)
        {
            var next = GoalSteps.Dequeue();
            var direction = RoomHelpers.GetDirectionForNextStep(Point, next);

            Direction = direction;
            DirectionHead = direction;
            
            StatusMap[RoomUserStatus.Move] = $"{next.X},{next.Y},{Math.Round(next.Z * 100.0) / 100.0}";

            NextPoint = next;
        }
        else
        {
            NextPoint = null;
            IsWalking = false;

            StatusMap.Remove(RoomUserStatus.Move);
        }
    }

    public void Dispose()
    {
        if (!_roomUserRepository.TryRemove(Id))
        {
            _logger.LogError($"Failed to dispose room user {Id}");
        }
    }
}