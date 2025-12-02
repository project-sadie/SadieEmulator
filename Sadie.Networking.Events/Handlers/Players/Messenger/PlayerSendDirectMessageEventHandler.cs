using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Player;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Constants;
using Sadie.Core.Shared.Extensions;
using Sadie.Db;
using Sadie.Db.Models.Players;
using Sadie.Networking.Packets.Writers.Players.Messenger;

namespace Sadie.Networking.Events.Handlers.Players.Messenger;

[PacketId(EventHandlerId.PlayerSendDirectMessage)]
public class PlayerSendDirectMessageEventHandler(
    IPlayerRepository playerRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper)
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

        var playerMessage = new PlayerMessageDto
        {
            OriginPlayerId = client.Player.Player.Id,
            TargetPlayerId = targetPlayer.Player.Id,
            Message = message,
            CreatedAt = DateTime.Now
        };

        await targetPlayer.NetworkObject.WriteToStreamAsync(new PlayerDirectMessageWriter
        {
            Message = playerMessage
        });

        var entity = mapper.Map<PlayerMessage>(playerMessage);
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.PlayerMessages.Add(entity);
        await dbContext.SaveChangesAsync();
    }
}