using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Doorbell;

namespace Sadie.Networking.Events.Handlers.Rooms.Doorbell;

public class RoomDoorbellAnswerEvent(IPlayerRepository playerRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var username = reader.ReadString();
        var accept = reader.ReadBool();

        if (!playerRepository.TryGetPlayerByUsername(username, out var player) || player == null)
        {
            return;
        }

        await player.NetworkObject.WriteToStreamAsync(accept
            ? new RoomDoorbellAcceptWriter("").GetAllBytes()
            : new RoomDoorbellNoAnswerWriter("").GetAllBytes());
    }
}

