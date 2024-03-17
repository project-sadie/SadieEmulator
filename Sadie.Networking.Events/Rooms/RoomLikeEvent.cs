using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Rooms;

public class RoomLikeEvent : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        // TODO
    }
}