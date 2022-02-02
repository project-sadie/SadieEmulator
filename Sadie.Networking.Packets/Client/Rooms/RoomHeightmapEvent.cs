using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;
using Sadie.Shared;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomHeightmapEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new RoomRelativeMapWriter(SadieConstants.MockHeightmap).GetAllBytes());
        await client.WriteToStreamAsync(new RoomHeightMapWriter(true, -1, SadieConstants.MockHeightmap.Replace("\r\n", "\r")).GetAllBytes());
    }
}