using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomLoadedEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInt();
        var password = reader.ReadString();

        await client.WriteToStreamAsync(new RoomLoadedWriter().GetAllBytes());
        await client.WriteToStreamAsync(new RoomDataWriter(roomId).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaintWriter("landscape", "0.0").GetAllBytes());
    }
}