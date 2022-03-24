using System.Drawing;
using Microsoft.Extensions.Logging;
using Sadie.Game.Rooms.Packets;
using Sadie.Shared;
using Sadie.Shared.Networking;
using Sadie.Shared.Extensions;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUser : RoomUserData, IRoomUser
{
    private readonly ILogger<RoomUser> _logger;
    private readonly Room _room;
    private readonly IRoomUserRepository _roomUserRepository;
    private readonly RoomConstants _constants;
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
        AvatarData avatarData,
        RoomConstants constants) : 
        base(id, point, directionHead, direction, avatarData)
    {
        _logger = logger;
        _room = room;
        _roomUserRepository = roomUserRepository;
        _constants = constants;
        NetworkObject = networkObject;
    }

    private void SetNextPosition()
    {
        Point = NextPoint ?? Point;
    }
    
    public void WalkToPoint(Point point, bool useDiagonal) // 2bMoved
    {
        StatusMap.Clear();
        
        SetNextPosition();
        
        GoalSteps = RoomHelpers.BuildPathForWalk(_room.Layout, new Point(Point.X, Point.Y), point, useDiagonal);
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

    private async Task ProcessMovementAsync() // 2bMoved
    {
        SetNextPosition();
        
        var doorPoint = _room.Layout.DoorPoint;
        
        if (NextPoint != null && Point.X == doorPoint.X && Point.Y == doorPoint.Y)
        {
            await LeaveRoomAsync();
            return;
        }

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

    public bool TrySpeak(string message, RoomChatBubble bubble, out RoomChatMessage? chatMesage)
    {
        if (string.IsNullOrEmpty(message) || message.Length > _constants.MaxChatMessageLength)
        {
            chatMesage = null;
            return false;
        }

        if (bubble == RoomChatBubble.Respect)
        {
            bubble = RoomChatBubble.Default;
        }
        
        chatMesage = new RoomChatMessage(this, message, _room, bubble, 1);
        return true;
    }

    private async Task SendToHotelViewAsync()
    {
        await NetworkObject.WriteToStreamAsync(new RoomUserHotelViewWriter().GetAllBytes());
    }

    public async Task LeaveRoomAsync()
    {
        await SendToHotelViewAsync();
        await DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (!_roomUserRepository.TryRemove(Id))
        {
            _logger.LogError($"Failed to dispose room user {Id}");
        }
        else
        {
            await _roomUserRepository.BroadcastDataAsync(new RoomUserLeftWriter(Id).GetAllBytes());
        }
    }
}