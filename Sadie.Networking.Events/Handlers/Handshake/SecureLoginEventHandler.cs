using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Enums.Game.Players;
using Sadie.Core.Shared;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Constants;
using Sadie.Db.Models.Server;
using Sadie.Networking.Packets.Writers.Handshake;
using Sadie.Options.Options;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerId.SecureLogin)]
public class SecureLoginEventHandler(
    ILogger<SecureLoginEventHandler> logger,
    IOptions<EncryptionOptions> encryptionOptions,
    IPlayerRepository playerRepository,
    ServerPlayerConstants constants,
    INetworkClientRepository networkClientRepository,
    ServerSettings serverSettings,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper,
    IPlayerLoaderService playerLoaderService,
    IPlayerHelperService playerHelperService,
    IConfiguration config)
    : INetworkPacketEventHandler
{
    public string? Token { get; set; }
    public int DelayMs { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var sw = Stopwatch.StartNew();

        if (DelayMs >= config.GetValue("PlayerOptions:MaxSsoDelayMs", 300_000))
        {
            await client.DisposeAsync();
            return;
        }

        if (string.IsNullOrEmpty(Token) || !ValidateSso(Token))
        {
            logger.LogWarning("Rejected an insecure sso token");
            await client.DisposeAsync();
            return;
        }
        
        if (encryptionOptions.Value.Enabled && !client.EncryptionEnabled)
        {
            logger.LogWarning("Encryption is enabled and TLS Handshake isn't finished.");
            await client.DisposeAsync();
            return;
        }

        var tokenRecord = await playerLoaderService.GetTokenAsync(Token, DelayMs);
        
        if (tokenRecord == null)
        {
            logger.LogWarning("Failed to find token record for provided sso.");
            await client.DisposeAsync();
            return;
        }
        
        var player = await playerRepository.GetPlayerByIdAsync(tokenRecord.PlayerId);

        if (player?.Data == null ||
            player.AvatarData == null ||
            player.NavigatorSettings == null ||
            player.GameSettings == null)
        {
            logger.LogError("Player record is missing required associated data.");
            await client.DisposeAsync();
            return;
        }
        
        if (player.Bans.Any(x => x.ExpiresAt == null || x.ExpiresAt >= DateTime.Now))
        {
            logger.LogWarning("Disconnected banned player {@PlayerUsername}", player.Username);
            await client.DisposeAsync();
            return;
        }

        var ipAddress = client
            .IpAddress
            .ToString()?
            .Split(":")
            .First() ?? "";
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        if (dbContext.BannedIpAddresses.Any(x => x.IpAddress == ipAddress && (x.ExpiresAt == null || x.ExpiresAt >= DateTime.Now)))
        {
            logger.LogWarning("Disconnected banned IP {@Ip}", ipAddress);
            await client.DisposeAsync();
            return;
        }
        
        var playerLogic = mapper.Map<IPlayerLogic>(player);

        playerLogic.NetworkObject = client;

        var playerId = player.Id;
        var existingPlayer = playerRepository.GetPlayerLogicById(playerId);

        if (existingPlayer?.NetworkObject != null)
        {
            await networkClientRepository.TryRemoveAsync(existingPlayer.NetworkObject.Guid);
        }

        if (!playerRepository.TryAddPlayer(playerLogic))
        {
            logger.LogError($"Player {playerLogic.Player.Username} could not be registered");
            await client.DisposeAsync();
            return;
        }
        
        await client.WriteToStreamAsync(new SecureLoginWriter());
        
        playerLogic.Player.Data.IsOnline = true;
        playerLogic.Player.Data.LastOnline = DateTime.Now;
        
        playerLogic.Authenticated = true;

        await NetworkPacketEventHelpers.SendLoginPacketsToPlayerAsync(client, playerLogic);
        await NetworkPacketEventHelpers.SendPlayerSubscriptionPacketsAsync(playerLogic);
        
        await playerHelperService.SendPlayerFriendListUpdate(playerLogic, playerRepository);

        var playersFriends = player.OutgoingFriendships
            .Concat(player.IncomingFriendships)
            .Where(x => x.Status == PlayerFriendshipStatus.Accepted);
        
        await playerHelperService.UpdatePlayerStatusForFriendsAsync(
            playerLogic, 
            playersFriends, 
            true, 
            false, 
            playerRepository);
        
        await SendWelcomeMessageAsync(playerLogic);

        client.Player = playerLogic;
        logger.LogInformation($"Player '{playerLogic.Player.Username}' has logged in from {ipAddress} ({Math.Round(sw.Elapsed.TotalMilliseconds)}ms)");
    }

    private async Task SendWelcomeMessageAsync(IPlayerLogic player)
    {
        if (string.IsNullOrEmpty(serverSettings.PlayerWelcomeMessage))
        {
            return;
        }

        var formattedMessage = serverSettings.PlayerWelcomeMessage
            .Replace("[username]", player.Player.Username)
            .Replace("[version]", GlobalState.Version.ToString());

        await player.SendAlertAsync(formattedMessage);
    }

    private bool ValidateSso(string sso) => sso.Length >= constants.MinSsoLength;
}