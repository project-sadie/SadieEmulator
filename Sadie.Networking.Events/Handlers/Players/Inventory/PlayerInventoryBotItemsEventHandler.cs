using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Players.Inventory;

[PacketId(EventHandlerId.PlayerInventoryBotItems)]
public class PlayerInventoryBotItemsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new PlayerInventoryBotItemsWriter
        {
            Bots = client.Player.Bots.Where(x => x.RoomId is null).ToList()
        });
    }
}