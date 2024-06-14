using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

[PacketId(EventHandlerIds.PlayerInventoryFurnitureItems)]
public class PlayerInventoryFurnitureItemsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var furnitureItems = client
            .Player
            .FurnitureItems
            .Where(x => !x.PlacementData.Any())
            .ToList();

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