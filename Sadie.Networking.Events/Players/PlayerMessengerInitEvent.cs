using Sadie.Game.Players;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Players;

public class PlayerMessengerInitEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IRoomRepository _roomRepository;
    private readonly PlayerConstants _playerConstants;

    public PlayerMessengerInitEvent(IPlayerRepository playerRepository, IRoomRepository roomRepository, PlayerConstants playerConstants)
    {
        _playerRepository = playerRepository;
        _roomRepository = roomRepository;
        _playerConstants = playerConstants;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerMessengerInitWriter(
            _playerConstants.MaxFriendships,
            1337, 
            _playerConstants.MaxFriendships,
            0
        ).GetAllBytes());

        var player = client.Player!;
        var friends = player.Data.FriendshipComponent.Friendships;
        var pages = friends.Count / 500 + 1;
        
        for (var i = 0; i < pages; i++)
        {
            var batch = friends.Skip(i * 500).
                Take(500).
                ToList();
            
            await client.WriteToStreamAsync(new PlayerFriendsListWriter(pages, i, batch, _playerRepository, _roomRepository).GetAllBytes());
        }
    }
}