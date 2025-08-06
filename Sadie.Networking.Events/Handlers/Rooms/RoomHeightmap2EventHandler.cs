using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms;

[PacketId(EventHandlerId.RoomHeightmap2)]
public class RoomHeightmap2EventHandler(RoomHeightmapEventHandler eventHandler) : INetworkPacketEventHandler
{
    // Nitro sends a different header based on if the user is exiting a room to enter another
    // Just call the original / other event
    
    public async Task HandleAsync(INetworkClient client)
    {
        await eventHandler.HandleAsync(client);
    }
}