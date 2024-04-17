using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Rooms;

public class RoomHeightmap2EventHandler(RoomHeightmapEventHandler eventHandler) : INetworkPacketEventHandler
{
    // Nitro sends a different header based on if the user is exiting a room to enter another
    // Just call the original / other event
    
    public int Id => EventHandlerIds.RoomHeightmap2;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await eventHandler.HandleAsync(client, reader);
    }
}