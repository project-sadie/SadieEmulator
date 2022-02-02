using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomHeightmapEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new RoomRelativeMap().GetAllBytes());
        await client.WriteToStreamAsync(new RoomHeightMap().GetAllBytes());
    }
}