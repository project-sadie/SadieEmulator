using System.Diagnostics;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Sadie.API.Game.Players;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Db.Models.Constants;
using Sadie.Db.Models.Server;
using Sadie.Shared.Attributes;
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
    IDbContextFactory<SadieContext> dbContextFactory,
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
            logger.LogError("Failed to resolve player record.");
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
            .Channel
            .RemoteAddress
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
        playerLogic.Channel = client.Channel;

        var playerId = player.Id;
        var existingPlayer = playerRepository.GetPlayerLogicById(playerId);

        client.Player = playerLogic;

        if (existingPlayer is { Channel: not null })
        {
            await playerRepository.TryRemovePlayerAsync(existingPlayer.Id);
            await networkClientRepository.TryRemoveAsync(existingPlayer.Channel.Id);

            var roomUser = client.RoomUser;
            
            if (roomUser != null)
            {
                await roomUser.Room.UserRepository.TryRemoveAsync(roomUser.Player.Id);
            }
        }

        if (!playerRepository.TryAddPlayer(playerLogic))
        {
            logger.LogError($"Player {playerLogic.Username} could not be registered");
            await client.DisposeAsync();
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
}