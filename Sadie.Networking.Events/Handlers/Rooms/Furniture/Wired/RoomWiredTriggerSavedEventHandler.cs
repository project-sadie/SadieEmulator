using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture.Wired;

[PacketId(EventHandlerId.RoomWiredTriggerSaved)]
public class RoomWiredTriggerSavedEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public required int ItemId { get; set; }
    public required List<int> Parameters { get; set; }
    public required string Input { get; set; }
    public required List<int> ItemIds { get; set; }
    public required int SelectionCode { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.RoomUser == null || 
            !client.RoomUser.HasRights())
        {
            return;
        }

        var room = client.RoomUser?.Room;
        var playerItem = client.Player.FurnitureItems.FirstOrDefault(x => x.Id == ItemId);

        if (playerItem == null)
        {
            return;
        }

        var selectedItems = room!
            .FurnitureItems
            .Where(x => ItemIds.Contains(x.PlayerFurnitureItem.Id))
            .Select(x => x.PlayerFurnitureItem)
            .ToList();

        playerItem.WiredData = new PlayerFurnitureItemWiredData
        {
            PlayerFurnitureItem = playerItem,
            SelectedItems = selectedItems,
            Message = Input,
        };

        dbContext.Entry(playerItem).State = EntityState.Unchanged;
        
        await client.WriteToStreamAsync(new WiredSavedWriter());
        await dbContext.SaveChangesAsync();
    }
}