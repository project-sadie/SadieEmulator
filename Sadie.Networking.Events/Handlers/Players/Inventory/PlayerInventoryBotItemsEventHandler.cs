using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

[PacketId(EventHandlerIds.PlayerInventoryBotItems)]
public class PlayerInventoryBotItemsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerInventoryBotItemsWriter
        {
            Bots = client.Player.Bots.Where(x => x.RoomId is null).ToList()
        });
    }
}