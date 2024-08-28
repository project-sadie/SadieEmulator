using Sadie.API.Game.Rooms.Services;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Attributes;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture.Wired;

[PacketId(EventHandlerId.RoomWiredEffectSaved)]
public class RoomWiredEffectSavedEventHandler(
    SadieContext dbContext,
    IRoomWiredService wiredService) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    public required List<int> Parameters { get; set; }
    public required string Input { get; set; }
    public required List<int> ItemIds { get; set; }
    public required int Delay { get; set; }
    public required int SelectionCode { get; set; }
    
    [RequiresRoomRights] 
    public async Task HandleAsync(INetworkClient client)
    {
        var room = client.RoomUser?.Room;
        
        var playerItem = client
            .Player
            .FurnitureItems
            .FirstOrDefault(x => x.Id == ItemId);

        if (playerItem?.PlacementData == null)
        {
            return;
        }

        var selectedItems = room!
            .FurnitureItems
            .Where(x => ItemIds.Contains(x.PlayerFurnitureItem.Id))
            .Select(x => x.PlayerFurnitureItem)
            .ToList();

        await wiredService.SaveSettingsAsync(
            playerItem.PlacementData,
            dbContext,
            new PlayerFurnitureItemWiredData
            {
                PlayerFurnitureItemPlacementDataId = playerItem.PlacementData.Id,
                PlacementData = playerItem.PlacementData,
                SelectedItems = selectedItems,
                Message = Input,
                Delay = Delay
            });

        await client.WriteToStreamAsync(new WiredSavedWriter());
    }
}