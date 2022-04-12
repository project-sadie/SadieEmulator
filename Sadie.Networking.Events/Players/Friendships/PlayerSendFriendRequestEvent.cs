using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friends;
using Sadie.Networking.Writers.Players.Friendships;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerSendFriendRequestEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerFriendshipRepository _friendshipRepository;
    private readonly PlayerConstants _playerConstants;

    public PlayerSendFriendRequestEvent(IPlayerRepository playerRepository, IPlayerFriendshipRepository friendshipRepository, PlayerConstants playerConstants)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
        _playerConstants = playerConstants;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var targetUsername = reader.ReadString();
        
        if (player!.FriendshipComponent.Friendships.Count >= _playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TooManyFriends).GetAllBytes());
            return;
        }
        
        if (targetUsername == client.Player!.Username)
        {
            return;
        }
        
        IPlayerData? targetData = null;
        var targetOnline = false;

        if (_playerRepository.TryGetPlayerByUsername(targetUsername, out var targetPlayer))
        {
            targetData = targetPlayer!;
            targetOnline = true;
        }
        else
        {
            var (found, offlineData) = await _playerRepository.TryGetPlayerDataByUsername(targetUsername);

            if (found)
            {
                targetData = offlineData!;
            }
        }
        
        if (targetData == null)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetNotFound).GetAllBytes());
            return;
        }

        if (player.FriendshipComponent.IsFriendsWith(targetData.Id))
        {
            return;
        }
        
        if (targetData.FriendshipComponent.Friendships.Count >= _playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetTooManyFriends).GetAllBytes());
        }
        
        if (!targetData.AllowFriendRequests)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetNotAccepting).GetAllBytes());
            return;
        }

        var playerFriends = player.FriendshipComponent.Friendships;
        var friendship = playerFriends.FirstOrDefault(x => x.TargetData.Username == targetUsername);

        if (friendship != null)
        {
            switch (friendship.Status)
            {
                case PlayerFriendshipStatus.Accepted:
                    return;
                case PlayerFriendshipStatus.Pending:
                    await _friendshipRepository.AcceptFriendRequestAsync(player.Id, targetData.Id);
                    player.FriendshipComponent.AcceptIncomingRequest(targetData.Id);

                    if (targetOnline && targetPlayer != null)
                    {
                        targetPlayer.FriendshipComponent.OutgoingRequestAccepted(targetData.Id);
                    }
                    return;
            }
        }
        
        await _friendshipRepository.CreateFriendRequestAsync(player.Id, targetData.Id);
        player.FriendshipComponent.Friendships = await _friendshipRepository.GetAllRecordsForPlayerAsync(client.Player.Id);

        if (targetOnline && targetPlayer != null)
        {
            targetPlayer.FriendshipComponent.Friendships = await _friendshipRepository.GetAllRecordsForPlayerAsync(targetPlayer.Id);
            await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerFriendRequestWriter(player.Id, player.Username, player.FigureCode).GetAllBytes());
        }
    }
}