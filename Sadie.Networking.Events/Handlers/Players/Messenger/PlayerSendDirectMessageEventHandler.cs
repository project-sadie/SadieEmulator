using Sadie.Database;
using Sadie.Database.Models.Players;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared;
using Sadie.Shared.Extensions;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerId.PlayerSendDirectMessage)]
public class PlayerSendDirectMessageEventHandler(
    PlayerRepository playerRepository,
    SadieContext dbContext)
    : INetworkPacketEventHandler
{
    public int PlayerId { get; set; }
    public required string Message { get; set; }

    public async Task HandleAsync(INetworkClient client)
    {
        if ((DateTime.Now - client.Player.State.LastDirectMessage).TotalMilliseconds < CooldownIntervals.PlayerDirectMessage)
        {
            return;
        }
        
        client.Player.State.LastDirectMessage = DateTime.Now;

        var playerId = PlayerId;
        var message = Message;

        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        message = message.Truncate(500);

        if (!client.Player.IsFriendsWith(PlayerId))
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
            Message = message,
            CreatedAt = DateTime.Now
        };

        await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerDirectMessageWriter
        {
            Message = playerMessage
        });
        
        dbContext.PlayerMessages.Add(playerMessage);
        await dbContext.SaveChangesAsync();
    }
}