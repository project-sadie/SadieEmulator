using Sadie.Game.Players.Inventory;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Players.Inventory;

public static class Idfk
{
    public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
        this IEnumerable<TSource> source, int size)
    {
        TSource[] bucket = null;
        var count = 0;

        foreach (var item in source)
        {
            if (bucket == null)
                bucket = new TSource[size];

            bucket[count++] = item;
            if (count != size)
                continue;

            yield return bucket;

            bucket = null;
            count = 0;
        }

        if (bucket != null && count > 0)
            yield return bucket.Take(count).ToArray();
    }
}
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

        var page = 0;
        var pages = (furnitureItems.Count() - 1) / 700 + 1;
        
        foreach (var batch in furnitureItems.Batch(700))
        {
            await client.WriteToStreamAsync(new PlayerInventoryFurnitureItemsWriter(pages, page, batch.ToList()).GetAllBytes());
            page++;
        }
    }
}