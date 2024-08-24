using System.Drawing;
using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Mapping;
using Sadie.API.Game.Rooms.Services;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Constants;
using Sadie.Enums.Game.Rooms;
using Sadie.Enums.Game.Rooms.Users;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Packets.Writers.Users;
using Sadie.Game.Rooms.Packets.Writers.Users.HandItems;
using Sadie.Game.Rooms.PathFinding;
using Sadie.Game.Rooms.Unit;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Game.Rooms.Users;

public class RoomUser(
    int id,
    RoomLogic room,
    INetworkObject networkObject,
    Point point,
    double pointZ,
    HDirection directionHead,
    HDirection direction,
    IPlayerLogic player,
    ServerRoomConstants roomConstants,
    RoomControllerLevel controllerLevel,
    IRoomTileMapHelperService tileMapHelperService,
    IRoomHelperService roomHelperService)
    : RoomUnitData(id, room, point, pointZ, directionHead, direction, tileMapHelperService),
        IRoomUser
{
    public IPlayerLogic Player { get; } = player;
    public DateTime LastAction { get; set; } = DateTime.Now;
    public TimeSpan IdleTime { get; } = TimeSpan.FromSeconds(roomConstants.SecondsTillUserIdle);
    public bool IsIdle { get; set; }
    public bool MoonWalking { get; set; }
    public IRoomUserTrade? Trade { get; set; }
    public int TradeStatus { get; set; }
    public int ActiveEffectId { get; set; }
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

        await ProcessGenericChecksAsync();
        await UpdateIdleStatusAsync();
        await UpdateEffectAsync();
    }

    private async Task UpdateEffectAsync()
    {
        var effectPointToCheck = IsWalking && NextPoint != null ? 
            NextPoint.Value : 
            Point;
        
        if (room.TileMap.EffectMap[effectPointToCheck.Y, effectPointToCheck.X] != 0)
        {
            var effectId = room.TileMap.EffectMap[Point.Y, Point.X];
            await SetEffectAsync((RoomUserEffect) effectId);
        }
        else if (ActiveEffectId != 0)
        {
            await SetEffectAsync(0);
        }
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

    public async Task SendWhisperAsync(string message)
    {
        await NetworkObject.WriteToStreamAsync(new RoomUserWhisperWriter
        {
            SenderId = Id,
            Message = message,
            EmotionId = (int) roomHelperService.GetEmotionFromMessage(message),
            Bubble = 0,
            Unknown = 0
        });
    }

    public async Task SetEffectAsync(RoomUserEffect effect)
    {
        ActiveEffectId = (int) effect;
        
        var writer = new RoomUserEffectWriter
        {
            UserId = Id,
            EffectId = (int) effect,
            DelayMs = 0
        };

        await Room.UserRepository.BroadcastDataAsync(writer);
    }
    public async ValueTask DisposeAsync()
    {
        room.TileMap.UserMap[Point].Remove(this);
    }
}