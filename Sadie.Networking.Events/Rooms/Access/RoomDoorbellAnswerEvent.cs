using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Access;

namespace Sadie.Networking.Events.Rooms.Access;

public class RoomDoorbellAnswerEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;

    public RoomDoorbellAnswerEvent(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var username = reader.ReadString();
        var accept = reader.ReadBool();

        if (!_playerRepository.TryGetPlayerByUsername(username, out var player) || player == null)
        {
            return;
        }

        await player.NetworkObject.WriteToStreamAsync(accept
            ? new RoomDoorbellAcceptWriter("").GetAllBytes()
            : new RoomDoorbellNoAnswerWriter("").GetAllBytes());
    }
}

