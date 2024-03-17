using Sadie.Game.Players.Inventory;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Players.Inventory;

public class PlayerInventoryFurnitureItemsEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var furnitureItems = client.Player.Data.Inventory.Items;

        if (furnitureItems.Count == 0)
        {
            await client.WriteToStreamAsync(new PlayerInventoryFurnitureItemsWriter(1, 0, new List<PlayerInventoryFurnitureItem>()).GetAllBytes());
            return;
        }

        var batchSize = 1000;
        var pages = (furnitureItems.Count - 1) / batchSize + 1;
        
        for (var i = 0; i < pages; i++)
        {
            var batch = furnitureItems.Skip(i * batchSize)
                .Take(batchSize)
                .ToList();

            await client.WriteToStreamAsync(new PlayerInventoryFurnitureItemsWriter(pages, i - 1, batch).GetAllBytes());
        }
    }
}