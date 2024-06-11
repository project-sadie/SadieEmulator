using System.Text;
using System.Text.RegularExpressions;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Generic;
using Sadie.Networking.Writers.Rooms;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.FloorPlanEditor;

[PacketId(EventHandlerIds.FloorPlanEditorSave)]
public partial class FloorPlanEditorSaveEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required string HeightMap { get; set; }
    public required int DoorX { get; set; }
    public required int DoorY { get; set; }
    public required int DoorRotation { get; set; }
    public required int WallSize { get; set; }
    public required int FloorSize { get; set; }
    public required int WallHeight { get; set; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (room.OwnerId != client.Player.Id)
        {
            return;
        }

        var errors = new List<string>();

        if (!ValidHeightMap().IsMatch(HeightMap))
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

        if (DoorRotation is < 0 or > 7)
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
        
        if (errors.Any())
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
        
        // TODO; Update

        await room.UserRepository.BroadcastDataAsync(new RoomForwardEntryWriter
        {
            RoomId = room.Id
        });
    }

    [GeneratedRegex("[a-zA-Z0-9\r]+")]
    private static partial Regex ValidHeightMap();
}