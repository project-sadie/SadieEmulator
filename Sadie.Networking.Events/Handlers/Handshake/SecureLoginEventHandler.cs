using System.Diagnostics;
using AutoMapper;
using DotNetty.Transport.Channels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Server;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Handshake;
using Sadie.Networking.Writers.Players.Other;
using Sadie.Options.Options;
using Sadie.Shared;
using Serilog;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerId.SecureLogin)]
public class SecureLoginEventHandler(
    ILogger<SecureLoginEventHandler> logger,
    IOptions<EncryptionOptions> encryptionOptions,
    PlayerRepository playerRepository,
    ServerPlayerConstants constants,
    INetworkClientRepository networkClientRepository,
    ServerSettings serverSettings,
    SadieContext dbContext,
    IMapper mapper)
    : INetworkPacketEventHandler
{
    public string? Token { get; set; }
    public int DelayMs { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var sw = Stopwatch.StartNew();

        if (string.IsNullOrEmpty(Token) || !ValidateSso(Token))
        {
            logger.LogWarning("Rejected an insecure sso token");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        if (encryptionOptions.Value.Enabled && !client.EncryptionEnabled)
        {
            logger.LogWarning("Encryption is enabled and TLS Handshake isn't finished.");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }

        var player = await PlayerLoader.LoadPlayerAsync(dbContext, Token, DelayMs);

        if (player == null)
        {
            logger.LogError("Failed to resolve player record.");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        var playerLogic = mapper.Map<PlayerLogic>(player);

        playerLogic.NetworkObject = client;
        playerLogic.Channel = client.Channel;

        var playerId = player.Id;
        var alreadyOnline = playerRepository.GetPlayerLogicById(playerId) != null;

        client.Player = playerLogic;

        if (alreadyOnline)
        {
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }

        if (!playerRepository.TryAddPlayer(playerLogic))
        {
            logger.LogError($"Player {playerLogic.Username} could not be registered");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        playerLogic.Data.IsOnline = true;
        playerLogic.Data.LastOnline = DateTime.Now;
        
        playerLogic.Authenticated = true;

        await NetworkPacketEventHelpers.SendLoginPacketsToPlayerAsync(client, playerLogic);
        await NetworkPacketEventHelpers.SendPlayerSubscriptionPacketsAsync(playerLogic);
        
        await PlayerHelpersToClean.SendPlayerFriendListUpdate(playerLogic, playerRepository);
        
        await PlayerHelpersToClean.UpdatePlayerStatusForFriendsAsync(
            player, 
            player.GetMergedFriendships(), 
            true, 
            false, 
            playerRepository);
        
        await SendWelcomeMessageAsync(playerLogic);
        
        await client.WriteToStreamAsync(new SecureLoginWriter());
        await client.WriteToStreamAsync(new PlayerPingWriter());
        
        logger.LogInformation($"Player '{playerLogic.Username}' has logged in ({Math.Round(sw.Elapsed.TotalMilliseconds)}ms)");
    }

    private async Task SendWelcomeMessageAsync(PlayerLogic player)
    {
        if (string.IsNullOrEmpty(serverSettings.PlayerWelcomeMessage))
        {
            return;
        }

        var formattedMessage = serverSettings.PlayerWelcomeMessage
            .Replace("[username]", player.Username)
            .Replace("[version]", GlobalState.Version.ToString());

        await player.SendAlertAsync(formattedMessage);
    }

    private bool ValidateSso(string sso) => sso.Length >= constants.MinSsoLength;

    private async Task DisconnectNetworkClientAsync(IChannelId channelId)
    {
        if (!await networkClientRepository.TryRemoveAsync(channelId))
        {
            logger.LogError("Failed to remove network client");
        }
    }
}