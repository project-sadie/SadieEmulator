using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerIds.PlayerSendDirectMessage)]
public class PlayerSendDirectMessageEventHandler(
    PlayerSendDirectMessageEventParser eventParser,
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
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

        if (!client.Player.IsFriendsWith(eventParser.PlayerId))
        {
            await client.WriteToStreamAsync(new PlayerMessageErrorWriter
            {
                Error = (int) PlayerMessageError.NotFriends,
                TargetId = playerId
            });
            
            return;
        }

        var targetPlayer = playerRepository.GetPlayerLogicById(playerId);
        
        if (targetPlayer == null)
        {
            return;
        }

        var playerMessage = new PlayerMessage
        {
            OriginPlayerId = client.Player.Id,
            TargetPlayerId = targetPlayer.Id,
            Message = message
        };

        client.Player.MessagesSent.Add(playerMessage);
        targetPlayer.MessagesReceived.Add(playerMessage);

        await dbContext.SaveChangesAsync();

        await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerDirectMessageWriter
        {
            Message = playerMessage
        });
    }
}