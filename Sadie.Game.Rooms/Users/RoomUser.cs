using System.Drawing;
using Microsoft.Extensions.Logging;
using Sadie.Shared.Networking;
using Sadie.Shared;

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
        Point = _nextPoint ?? Point;
    }
    
    public void WalkToPoint(Point point, bool useDiagonal) // 2bMoved
    {
        SetNextPosition();
        
        _goalSteps = RoomHelpers.BuildPathForWalk(_room.Layout, new Point(Point.X, Point.Y), point, useDiagonal);
        _isWalking = true;
    }

    public async Task RunPeriodicCheckAsync()
    
    {
        Console.WriteLine($"Checking on {Username}");
        
        if (_isWalking)
        {
            await ProcessMovementAsync();
        }
    }

    private async Task ProcessMovementAsync() // 2bMoved
    {
        SetNextPosition();

        if (_goalSteps.Count > 0)
        {
            var next = _goalSteps.Dequeue();
            var direction = RoomHelpers.GetDirectionForNextStep(Point, next);

            Direction = direction;
            DirectionHead = direction;
            
            StatusMap[RoomUserStatus.Move] = $"{next.X},{next.Y},{Math.Round(next.Z * 100.0) / 100.0}";

            _nextPoint = next;
        }
        else
        {
            _nextPoint = null;
            _isWalking = false;

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