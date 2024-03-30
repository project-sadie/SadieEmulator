using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Doorbell;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

public class RoomDoorbellAnswerEvent(
    RoomDoorbellAnswerParser parser,
    IPlayerRepository playerRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(
        INetworkClient client, 
        INetworkPacketReader reader)
    {
        parser.Parse(reader);

        var username = parser.Username;

        if (!playerRepository.TryGetPlayerByUsername(username, out var player) || player == null)
        {
            return;
        }

        await player.NetworkObject.WriteToStreamAsync(parser.Accept
            ? new RoomDoorbellAcceptWriter(username).GetAllBytes()
            : new RoomDoorbellNoAnswerWriter(username).GetAllBytes());
    }
}

