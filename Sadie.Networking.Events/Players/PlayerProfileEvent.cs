using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Players;

public class PlayerProfileEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerFriendshipRepository _friendshipRepository;

    public PlayerProfileEvent(IPlayerRepository playerRepository, IPlayerFriendshipRepository friendshipRepository)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInt();
        var playerOnline = false;

        IPlayerData playerData = null;
        
        if (playerId == client.Player.Id)
        {
            playerData = client.Player;
            playerOnline = true;
        }
        else if (_playerRepository.TryGetPlayerById(playerId, out var player))
        {
            playerData = player;
            playerOnline = true;
        }
        else
        {
            var (found, fetchedPlayerData) = await _playerRepository.TryGetPlayerData(playerId);

            if (found)
            {
                playerData = fetchedPlayerData;
            }
        }

        if (playerData == null)
        {
            return;
        }

        var friendCount = playerData.FriendshipComponent.Friendships.Count;
        var friendshipExists = await _friendshipRepository.DoesFriendshipExist(client.Player.Id, playerId);
        var friendshipRequestExists = await _friendshipRepository.DoesRequestExist(client.Player.Id, playerId);
        
        await client.WriteToStreamAsync(new PlayerProfileWriter(playerData, playerOnline, friendCount, friendshipExists, friendshipRequestExists).GetAllBytes());
    }
}