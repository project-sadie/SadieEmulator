using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

[PacketId(EventHandlerId.PlayerInventoryBotItems)]
public class PlayerInventoryBotItemsEventHandler(SadieContext dbContext) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null)
        {
            return;
        }

        client.Player.Bots ??= await dbContext
            .PlayerBots
            .Where(x => x.PlayerId == client.Player.Id)
            .ToListAsync();
        
        await client.WriteToStreamAsync(new PlayerInventoryBotItemsWriter
        {
            Bots = client
                .Player
                .Bots
                .Where(x => x.RoomId == null)
                .ToList()
        });
    }
}