using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Helpers;

namespace Sadie.Networking.Events.Handlers.Rooms.FloorPlanEditor;

[PacketId(EventHandlerId.FloorPlanEditorSave)]
public class FloorPlanEditorSaveEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory,
    IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required string HeightMap { get; init; }
    public required int DoorX { get; init; }
    public required int DoorY { get; init; }
    public required int DoorDirection { get; init; }
    public required int WallSize { get; init; }
    public required int FloorSize { get; init; }
    public required int WallHeight { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out _) ||
            room.OwnerId != client.Player.Id || 
            room.Layout == null)
        {
            return;
        }

        var errors = GetErrors();
        
        if (errors.Count != 0)
        {
            await client.WriteToStreamAsync(new BubbleAlertWriter
            {
                Key = EnumHelpers.GetEnumDescription(NotificationType.FloorPlanEditor)!,
                Messages = new Dictionary<string, string>
                {
                    { "message", string.Join("<br>", errors) }
                }
            });
            
            return;
        }

        var newLayout = false;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        if (!room.Layout.Name!.Contains("custom_"))
        {
            room.Layout = new RoomLayout
            {
                Name = $"custom_{Guid.NewGuid().ToString().Replace("-", "")[..15]}",
                DoorDirection = (HDirection)DoorDirection,
                DoorX = DoorX,
                DoorY = DoorY,
                HeightMap = HeightMap
            };
            
            dbContext.Entry(room.Layout).State = EntityState.Added;
            newLayout = true;
        }
        else
        {
            room.Layout.DoorDirection = (HDirection) DoorDirection;
            room.Layout.DoorX = DoorX;
            room.Layout.DoorY = DoorY;
            room.Layout.HeightMap = HeightMap;
            
            dbContext.Entry(room.Layout).State = EntityState.Modified;
        }

        await dbContext.SaveChangesAsync();

        if (newLayout)
        {
            room.LayoutId = room.Layout.Id;
            
            dbContext.Entry((Room) room).Property(x => x.LayoutId).IsModified = true;
            await dbContext.SaveChangesAsync();
        }

        var playersToForward = new List<IPlayerLogic>();

        foreach (var user in room.UserRepository.GetAll())
        {
            await room.UserRepository.TryRemoveAsync(user.Player.Id, false, true);
            playersToForward.Add(user.Player);
        }

        if (!roomRepository.TryRemove(room.Id, out var roomLogic))
        {
            return;
        }

        var writer = new RoomForwardEntryWriter
        {
            RoomId = roomLogic!.Id
        };

        foreach (var player in playersToForward)
        {
            if (player.NetworkObject == null)
            {
                continue;
            }
            
            await player.NetworkObject.WriteToStreamAsync(writer);
        }
    }

    public List<string> GetErrors()
    {
        var errors = new List<string>();

        if (!Regex.IsMatch(HeightMap, "[a-zA-Z0-9\r]+"))
        {
            errors.Add("${notification.floorplan_editor.error.title}");
        }

        if (HeightMap.Length > 64 * 64)
        {
            errors.Add("${notification.floorplan_editor.error.message.too_large_area}");
        }

        var rows = HeightMap.Split("\r");

        if (DoorX < 0 || DoorX > rows[0].Length || DoorY < 0 || DoorY >= rows.Length)
        {
            errors.Add("${notification.floorplan_editor.error.message.entry_tile_outside_map}");
        }

        if (DoorY < rows.Length && DoorX < rows[DoorY].Length && rows[DoorY][DoorX] == 'x')
        {
            errors.Add("${notification.floorplan_editor.error.message.entry_not_on_tile}");
        }

        if (DoorDirection is < 0 or > 7)
        {
            errors.Add("${notification.floorplan_editor.error.message.invalid_entry_tile_direction}");
        }

        if (WallSize is < -2 or > 1)
        {
            errors.Add("${notification.floorplan_editor.error.message.invalid_wall_thickness}");
        }

        if (FloorSize is < -2 or > 1)
        {
            errors.Add("${notification.floorplan_editor.error.message.invalid_floor_thickness}");
        }

        if (WallHeight is < -1 or > 15)
        {
            errors.Add("${notification.floorplan_editor.error.message.invalid_walls_fixed_height}");
        }

        return errors;
    }
}