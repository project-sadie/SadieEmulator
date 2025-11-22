using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Writers.Players.Inventory;

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