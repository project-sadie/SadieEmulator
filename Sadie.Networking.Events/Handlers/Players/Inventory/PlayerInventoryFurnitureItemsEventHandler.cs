using Sadie.Game.Players.Inventory;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

public class PlayerInventoryFurnitureItemsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var furnitureItems = client.Player.Data.Inventory.Items;

        if (furnitureItems.Count == 0)
        {
            await client.WriteToStreamAsync(new PlayerInventoryFurnitureItemsWriter(1, 0, new List<PlayerInventoryFurnitureItem>()).GetAllBytes());
            return;
        }

        var page = 0;
        var pages = (furnitureItems.Count() - 1) / 700 + 1;
        
        foreach (var batch in furnitureItems.Batch(700))
        {
            await client.WriteToStreamAsync(new PlayerInventoryFurnitureItemsWriter(pages, page, batch.ToList()).GetAllBytes());
            page++;
        }
    }
}