using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Enums.Game.Players;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players.Messenger;
using Sadie.Shared.Constants;
using Sadie.Shared.Extensions;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerId.PlayerSendDirectMessage)]
public class PlayerSendDirectMessageEventHandler(
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory)
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
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerMessages.Add(playerMessage);
        await dbContext.SaveChangesAsync();
    }
}