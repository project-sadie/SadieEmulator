using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Bots;

[PacketId(EventHandlerIds.RoomPlayerBotPlaced)]
public class RoomPlayerBotPlacedEventHandler(RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public required int Id { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var bot = client.Player.Bots.FirstOrDefault(x => x.Id == Id);

        if (bot == null)
        {
            return;
        }

        if (!room.BotRepository.TryAdd(bot))
        {
            // TODO; handle
        }
        
        //
    }
}