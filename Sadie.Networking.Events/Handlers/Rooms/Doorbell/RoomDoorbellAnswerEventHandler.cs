using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Doorbell;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

public class RoomDoorbellAnswerEventHandler(
    RoomDoorbellAnswerEventParser eventParser,
    IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.RoomDoorbellAnswer;

    public async Task HandleAsync(
        INetworkClient client, 
        INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var username = eventParser.Username;

        if (!playerRepository.TryGetPlayerByUsername(username, out var player) || player == null)
        {
            return;
        }

        await player.NetworkObject.WriteToStreamAsync(eventParser.Accept
            ? new RoomDoorbellAcceptWriter(username).GetAllBytes()
            : new RoomDoorbellNoAnswerWriter(username).GetAllBytes());
    }
}

