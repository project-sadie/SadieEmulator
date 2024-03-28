using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Messenger;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Players.Messenger;

public class PlayerSendDirectMessageEvent(IPlayerRepository playerRepository, IPlayerMessageDao playerMessageDao)
    : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if ((DateTime.Now - client.Player.State.LastDirectMessage).TotalMilliseconds < CooldownIntervals.PlayerDirectMessage)
        {
            return;
        }
        
        client.Player.State.LastDirectMessage = DateTime.Now;
        
        var playerId = reader.ReadInteger();
        var message = reader.ReadString();

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

        if (friend == null || friend.Status != PlayerFriendshipStatus.Accepted)
        {
            await client.WriteToStreamAsync(new PlayerMessageErrorWriter(PlayerMessageError.NotFriends, playerId).GetAllBytes());
            return;
        }

        if (!playerRepository.TryGetPlayerById(playerId, out var targetPlayer) || targetPlayer == null)
        {
            return;
        }

        var playerMessage = new PlayerMessage(client.Player.Data.Id, targetPlayer.Data.Id, message);

        await playerMessageDao.CreateMessageAsync(playerMessage);
        await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerDirectMessageWriter(playerMessage).GetAllBytes());
    }
}