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

    public PlayerSendMessageEvent(IPlayerRepository playerRepository)
    {
        _playerRepository = playerRepository;
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
        
        var friend =
            client.Player.Data.FriendshipComponent.Friendships.FirstOrDefault(x => x.TargetData.Id == playerId);

        if (friend == null)
        {
            // GetClient().SendPacket(new InstantMessageErrorComposer(MessengerMessageErrors.NotFriends, ToId));
            return;
        }

        if (!_playerRepository.TryGetPlayerById(playerId, out var targetPlayer) || targetPlayer == null)
        {
            Console.WriteLine("cant find target");
            return;
        }

        var playerMessage = new PlayerMessage(client.Player.Data.Id, targetPlayer.Data.Id, message);
        
        // TODO: Store it in database
        
        await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerMessageWriter(playerMessage).GetAllBytes());
    }
}