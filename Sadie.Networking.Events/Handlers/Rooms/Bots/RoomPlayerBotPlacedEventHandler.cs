using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Bots;

[PacketId(EventHandlerIds.RoomPlayerBotPlaced)]
public class RoomPlayerBotPlacedEventHandler : INetworkPacketEventHandler
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}