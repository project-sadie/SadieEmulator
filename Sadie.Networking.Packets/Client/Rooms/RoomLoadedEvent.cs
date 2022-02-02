using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Rooms;

namespace Sadie.Networking.Packets.Client.Rooms;

public class RoomLoadedEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomId = reader.ReadInt();
        var password = reader.ReadString();

        await client.WriteToStreamAsync(new LoadedRoom().GetAllBytes());
        await client.WriteToStreamAsync(new RoomInformation(roomId).GetAllBytes());
        await client.WriteToStreamAsync(new RoomPaint("landscape", "0.0").GetAllBytes());
    }
}