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
    public required int ItemId { get; init; }
    public required List<int> Parameters { get; init; }
    public required string Input { get; init; }
    public required List<int> ItemIds { get; init; }
    public required int SelectionCode { get; init; }
    
    [RequiresRoomRights] 
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser?.Room;

        var roomItem = room?.FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItemId == ItemId);

        if (roomItem == null)
        {
            return;
        }

        var selectedItems = room!
            .FurnitureItems
            .Where(x => ItemIds.Contains(x.PlayerFurnitureItemId))
            .ToList();

        var parameters = Parameters
            .Select(x => new PlayerFurnitureItemWiredParameter
            {
                Value = x
            }).ToList();

        var wiredData = new PlayerFurnitureItemWiredData
        {
            PlayerFurnitureItemPlacementDataId = roomItem.Id,
            PlacementData = roomItem,
            Message = Input,
            PlayerFurnitureItemWiredParameters = parameters,
            PlayerFurnitureItemWiredItems = selectedItems.Select(x => new PlayerFurnitureItemWiredItem
            {
                PlayerFurnitureItemPlacementDataId = x.Id
            }).ToList()
        };

        await wiredService.SaveSettingsAsync(
            roomItem,
            dbContext,
            wiredData);
        
        await client.WriteToStreamAsync(new WiredSavedWriter());
    }
}