using System.Diagnostics;
using AutoMapper;
using DotNetty.Transport.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API.Game.Players;
using Sadie.Database;
using Sadie.Database.Models.Constants;
using Sadie.Database.Models.Server;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Handshake;
using Sadie.Options.Options;
using Sadie.Shared;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerId.SecureLogin)]
public class SecureLoginEventHandler(
    ILogger<SecureLoginEventHandler> logger,
    IOptions<EncryptionOptions> encryptionOptions,
    IPlayerRepository playerRepository,
    ServerPlayerConstants constants,
    INetworkClientRepository networkClientRepository,
    ServerSettings serverSettings,
    SadieContext dbContext,
    IMapper mapper,
    IPlayerLoaderService playerLoaderService,
    IPlayerHelperService playerHelperService)
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

        var tokenRecord = await playerLoaderService.GetTokenAsync(Token, DelayMs);
        
        if (tokenRecord == null)
        {
            logger.LogWarning("Failed to find token record for provided sso.");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        var player = await playerRepository.GetPlayerByIdAsync(tokenRecord.PlayerId);

        if (player == null)
        {
            logger.LogError("Failed to resolve player record.");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        if (player.Bans.Any(x => x.ExpiresAt == null || x.ExpiresAt >= DateTime.Now))
        {
            logger.LogWarning("Disconnected banned player {@PlayerUsername}", player.Username);
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }

        var ipAddress = client
            .Channel
            .RemoteAddress
            .ToString()?
            .Split(":")
            .First() ?? "";
        
        if (dbContext.BannedIpAddresses.Any(x => x.IpAddress == ipAddress && (x.ExpiresAt == null || x.ExpiresAt >= DateTime.Now)))
        {
            logger.LogWarning("Disconnected banned IP {@Ip}", ipAddress);
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        var playerLogic = mapper.Map<IPlayerLogic>(player);

        playerLogic.NetworkObject = client;
        playerLogic.Channel = client.Channel;

        var playerId = player.Id;
        var existingPlayer = playerRepository.GetPlayerLogicById(playerId);

        client.Player = playerLogic;

        if (existingPlayer != null)
        {
            await playerRepository.TryRemovePlayerAsync(existingPlayer.Id);
            await networkClientRepository.TryRemoveAsync(existingPlayer.Channel.Id);
        }

        if (!playerRepository.TryAddPlayer(playerLogic))
        {
            logger.LogError($"Player {playerLogic.Username} could not be registered");
            await DisconnectNetworkClientAsync(client.Channel.Id);
            return;
        }
        
        await client.WriteToStreamAsync(new SecureLoginWriter());
        
        playerLogic.Data.IsOnline = true;
        playerLogic.Data.LastOnline = DateTime.Now;
        
        playerLogic.Authenticated = true;

        await NetworkPacketEventHelpers.SendLoginPacketsToPlayerAsync(client, playerLogic);
        await NetworkPacketEventHelpers.SendPlayerSubscriptionPacketsAsync(playerLogic);
        
        await playerHelperService.SendPlayerFriendListUpdate(playerLogic, playerRepository);
        
        await playerHelperService.UpdatePlayerStatusForFriendsAsync(
            playerLogic, 
            player.GetMergedFriendships(), 
            true, 
            false, 
            playerRepository);
        
        await SendWelcomeMessageAsync(playerLogic);
        
        logger.LogInformation($"Player '{playerLogic.Username}' has logged in from {ipAddress} ({Math.Round(sw.Elapsed.TotalMilliseconds)}ms)");
    }

    private async Task SendWelcomeMessageAsync(IPlayerLogic player)
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