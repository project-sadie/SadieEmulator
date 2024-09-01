using Sadie.API.Game.Rooms.Services;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Attributes;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture.Wired;

[PacketId(EventHandlerId.RoomWiredTriggerSaved)]
public class RoomWiredTriggerSavedEventHandler(
    SadieContext dbContext,
    IRoomWiredService wiredService) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    public required List<int> Parameters { get; set; }
    public required string Input { get; set; }
    public required List<int> ItemIds { get; set; }
    public required int SelectionCode { get; set; }
    
    [RequiresRoomRights] 
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser?.Room;

        var roomItem = room?.FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);

        if (roomItem == null)
        {
            return;
        }

        var roomItems = room!
            .FurnitureItems
            .Where(x => ItemIds.Contains(x.PlayerFurnitureItem.Id))
            .ToList();

        await wiredService.SaveSettingsAsync(
            roomItem,
            dbContext,
            new PlayerFurnitureItemWiredData
            {
                PlayerFurnitureItemPlacementDataId = roomItem.Id,
                PlacementData = roomItem,
                SelectedItems = roomItems,
                Message = Input,
            });
        
        await client.WriteToStreamAsync(new WiredSavedWriter());
    }
}