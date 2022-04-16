using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Players.Effects;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;
using Sadie.Networking.Writers.Moderation;
using Sadie.Networking.Writers.Players;
using Sadie.Networking.Writers.Players.Clothing;
using Sadie.Networking.Writers.Players.Effects;
using Sadie.Networking.Writers.Players.Navigator;
using Sadie.Networking.Writers.Players.Other;
using Sadie.Networking.Writers.Players.Permission;
using Sadie.Networking.Writers.Players.Rooms;
using Sadie.Shared.Networking;

namespace Sadie.Networking.Events.Handshake;

public class SecureLoginEvent : INetworkPacketEvent
{
    private readonly ILogger<SecureLoginEvent> _logger;
    private readonly IPlayerRepository _playerRepository;
    private readonly PlayerConstants _constants;
    private readonly INetworkClientRepository _networkClientRepository;

    public SecureLoginEvent(ILogger<SecureLoginEvent> logger, IPlayerRepository playerRepository, PlayerConstants constants, INetworkClientRepository networkClientRepository)
    {
        _logger = logger;
        _playerRepository = playerRepository;
        _constants = constants;
        _networkClientRepository = networkClientRepository;
    }
        
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var sso = reader.ReadString();

        if (!ValidateSso(sso)) 
        {
            _logger.LogWarning("Rejected an insecure sso token");
            await DisconnectAsync(client.Guid);
            return;
        }
            
        var (foundPlayer, player) = await _playerRepository.TryGetPlayerBySsoAsync(client, sso);

        if (!foundPlayer || player == null) // put the second check to shut my IDE up about nullable markings.
        {
            _logger.LogWarning("Failed to resolve player from their provided sso");
            
            if (!await _networkClientRepository.TryRemoveAsync(client.Guid))
            {
                _logger.LogError("Failed to remove network client");
            }
            return;
        }

        var playerData = player.Data;
        var playerId = playerData.Id;
        
        await _playerRepository.ResetSsoTokenForPlayerAsync(playerId);
            
        client.Player = player;
        
        if (!_playerRepository.TryAddPlayer(player))
        {
            _logger.LogError($"Player {playerId} could not be registered");
            await DisconnectAsync(client.Guid);
            return;
        }
            
        _logger.LogInformation($"Player '{playerData.Username}' has logged in");
        await _playerRepository.MarkPlayerAsOnlineAsync(playerId);

        await _playerRepository.UpdateMessengerStatusForFriends(playerData.Id,
            playerData.FriendshipComponent.Friendships, true, false);

        player.Data.LastOnline = DateTime.Now;
        player.Authenticated = true;

        await SendExtraPacketsAsync(client, player.Data);
    }

    private static async Task SendExtraPacketsAsync(INetworkObject networkObject, IPlayerData playerData)
    {
        await networkObject.WriteToStreamAsync(new SecureLoginWriter().GetAllBytes());
        await networkObject.WriteToStreamAsync(new NoobnessLevelWriter(1).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerHomeRoomWriter(playerData.HomeRoom, playerData.HomeRoom).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerEffectListWriter(new List<PlayerEffect>()).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerClothingListWriter().GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerPermissionsWriter(1, 2, true).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerStatusWriter(true, false, true).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerNavigatorSettingsWriter(playerData.NavigatorSettings).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerNotificationSettingsWriter(playerData.Settings.ShowNotifications).GetAllBytes());
        await networkObject.WriteToStreamAsync(new PlayerAchievementScoreWriter(playerData.AchievementScore).GetAllBytes());

        if (playerData.Permissions.Contains("moderation_tools"))
        {
            await networkObject.WriteToStreamAsync(new ModerationToolsWriter().GetAllBytes());
        }
    }

    private bool ValidateSso(string sso) => 
        !string.IsNullOrEmpty(sso) && sso.Length >= _constants.MinSsoLength;

    public async Task DisconnectAsync(Guid guid)
    {
        if (!await _networkClientRepository.TryRemoveAsync(guid))
        {
            _logger.LogError("Failed to remove network client");
        }
    }
}