using Sadie.Game.Players;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Messenger;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players.Messenger;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

public class PlayerSendDirectMessageEventHandler(
    PlayerSendDirectMessageEventParser eventParser,
    PlayerRepository playerRepository, 
    IPlayerMessageDao playerMessageDao)
    : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerSendDirectMessage;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        if ((DateTime.Now - client.Player.State.LastDirectMessage).TotalMilliseconds < CooldownIntervals.PlayerDirectMessage)
        {
            return;
        }
        
        client.Player.State.LastDirectMessage = DateTime.Now;

        var playerId = eventParser.PlayerId;
        var message = eventParser.Message;

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