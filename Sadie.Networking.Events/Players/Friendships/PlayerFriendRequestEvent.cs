using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Friends;

namespace Sadie.Networking.Events.Players.Friendships;

public class PlayerFriendRequestEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerFriendshipRepository _friendshipRepository;
    private readonly PlayerConstants _playerConstants;

    public PlayerFriendRequestEvent(IPlayerRepository playerRepository, IPlayerFriendshipRepository friendshipRepository, PlayerConstants playerConstants)
    {
        _playerRepository = playerRepository;
        _friendshipRepository = friendshipRepository;
        _playerConstants = playerConstants;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var player = client.Player;
        var targetUsername = reader.ReadString();
        
        if (player!.Friendships.Count >= _playerConstants.MaxFriendships)
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
            var (found, offlineData) = await _playerRepository.TryGetPlayerData(targetUsername);

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
        
        if (targetData.Friendships.Count >= _playerConstants.MaxFriendships)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetTooManyFriends).GetAllBytes());
        }
        
        if (!targetData.AllowFriendRequests)
        {
            await client.WriteToStreamAsync(new PlayerFriendshipErrorWriter(0, PlayerFriendshipError.TargetNotAccepting).GetAllBytes());
            return;
        }

        var friendship = player.Friendships.FirstOrDefault(x => x.Username == targetUsername);

        if (friendship != null)
        {
            switch (friendship.Status)
            {
                case PlayerFriendshipStatus.Accepted:
                    return;
                case PlayerFriendshipStatus.Pending:
                    await _friendshipRepository.UpdateAsync(player.Id, targetData.Id, PlayerFriendshipStatus.Accepted);

                    var playersState = player.Friendships.FirstOrDefault(x => x.Username == targetUsername);

                    if (playersState != null)
                    {
                        playersState.Status = PlayerFriendshipStatus.Accepted;
                    }

                    if (targetOnline && targetPlayer != null)
                    {
                        var targetState = targetPlayer.Friendships.FirstOrDefault(x => x.Username == targetUsername);

                        if (targetState != null)
                        {
                            targetState.Status = PlayerFriendshipStatus.Accepted;
                        }
                    }
                    return;
                case PlayerFriendshipStatus.Declined:
                default:
                    break;
            }
        }
        
        await _friendshipRepository.CreateAsync(player.Id, targetData.Id, PlayerFriendshipStatus.Pending);

        if (targetOnline && targetPlayer != null)
        {
            await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerFriendRequestWriter(player.Id, player.Username, player.FigureCode).GetAllBytes());
        }
    }
}