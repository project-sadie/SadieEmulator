using Sadie.Game.Players;
using Sadie.Game.Players.Messenger;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Players.Messenger;

public class PlayerSendMessageEvent : INetworkPacketEvent
{
    private readonly IPlayerRepository _playerRepository;
    private readonly IPlayerMessageDao _playerMessageDao;

    public PlayerSendMessageEvent(IPlayerRepository playerRepository, IPlayerMessageDao playerMessageDao)
    {
        _playerRepository = playerRepository;
        _playerMessageDao = playerMessageDao;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var playerId = reader.ReadInteger();
        var message = reader.ReadString();
        
        // TODO: Cooldown

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        if (message.Length > 500)
        {
            message = message.Truncate(500);
        }

        var friendships = client.Player.Data.FriendshipComponent.Friendships;
        var friend = friendships.FirstOrDefault(x => x.TargetData.Id == playerId);

        if (friend == null)
        {
            await client.WriteToStreamAsync(new PlayerMessageErrorWriter(PlayerMessageError.NotFriends, playerId).GetAllBytes());
            return;
        }

        if (!_playerRepository.TryGetPlayerById(playerId, out var targetPlayer) || targetPlayer == null)
        {
            Console.WriteLine("cant find target");
            return;
        }

        var playerMessage = new PlayerMessage(client.Player.Data.Id, targetPlayer.Data.Id, message);

        await _playerMessageDao.CreateMessageAsync(playerMessage);
        await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerMessageWriter(playerMessage).GetAllBytes());
    }
}