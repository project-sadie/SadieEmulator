using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

public class PlayerInventoryFurnitureItemsEventHandler : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerInventoryFurnitureItems;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var furnitureItems = client.Player.FurnitureItems;

        if (furnitureItems.Count == 0)
        {
            await client.WriteToStreamAsync(new PlayerInventoryFurnitureItemsWriter
            {
                Pages = 1,
                CurrentPage = 0,
                Items = []
            });
            return;
        }

        var page = 0;
        var pages = (furnitureItems.Count - 1) / 700 + 1;
        
        foreach (var batch in furnitureItems.Batch(700))
        {
            await client.WriteToStreamAsync(new PlayerInventoryFurnitureItemsWriter
            {
                Pages = pages,
                CurrentPage = page,
                Items = batch.ToList()
            });
            
            page++;
        }
    }
}