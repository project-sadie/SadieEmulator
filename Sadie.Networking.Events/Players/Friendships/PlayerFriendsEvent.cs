using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerFriendsEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;

    public PlayerFriendsEvent(IPlayerRepository playerRepository, IRoomRepository roomRepository)
    {
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
    }
}