using System.Drawing;
using Sadie.Game.Rooms.Chat;
using Sadie.Shared.Networking;
using Sadie.Shared.Extensions;
using Sadie.Shared.Game.Avatar;
using Sadie.Shared.Game.Rooms;

namespace Sadie.Game.Rooms.Users;

public class RoomUser : RoomUserData, IRoomUser
{
    private readonly Room _room;
    private readonly RoomConstants _constants;
    
    public int Id { get; }
    public INetworkObject NetworkObject { get; }

    public RoomUser(
        Room room,
        INetworkObject networkObject, 
        int id, 
        HPoint point, 
        HDirection directionHead, 
        HDirection direction,
        AvatarData avatarData,
        RoomConstants constants) : 
        base(point, directionHead, direction, avatarData, TimeSpan.FromSeconds(constants.SecondsTillUserIdle))
    {
        _room = room;
        _constants = constants;
        
        Id = id;
        NetworkObject = networkObject;
    }

    private void SetNextPosition()
    {
        Point = NextPoint ?? Point;
    }
    
    public void WalkToPoint(Point point, bool useDiagonal)
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

    public bool TryCreateChatMessage(string message, RoomChatBubble bubble, out RoomChatMessage? chatMesage)
    {
        if (string.IsNullOrEmpty(message) || message.Length > _constants.MaxChatMessageLength)
        {
            chatMesage = null;
            return false;
        }
        
        chatMesage = new RoomChatMessage(this, message, _room, bubble, 1);
        return true;
    }

    public async ValueTask DisposeAsync()
    {
        
    }
}