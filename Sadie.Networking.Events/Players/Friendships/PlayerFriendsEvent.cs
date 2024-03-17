using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerFriendsEvent(IPlayerRepository playerRepository, IRoomRepository roomRepository)
    : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository = playerRepository;
    private readonly IRoomRepository _roomRepository = roomRepository;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
    }
}