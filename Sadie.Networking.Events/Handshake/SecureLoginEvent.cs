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

    public SecureLoginEvent(ILogger<SecureLoginEvent> logger, IPlayerRepository playerRepository, PlayerConstants constants)
    {
        _logger = logger;
        _playerRepository = playerRepository;
        _constants = constants;
    }
        
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var sso = reader.ReadString();

        if (!ValidateSso(sso)) 
        {
            _logger.LogWarning("Rejected an insecure sso token");
                
            await client.DisposeAsync();
            return;
        }
            
        var (foundPlayer, player) = await _playerRepository.TryGetPlayerBySsoAsync(client, sso);

        if (!foundPlayer || player == null) // put the second check to shut my IDE up about nullable markings.
        {
            _logger.LogWarning("Failed to resolve player from their provided sso");
                
            await client.DisposeAsync();
            return;
        }
        
        await _playerRepository.ResetSsoTokenForPlayerAsync(player.Id);
            
        client.Player = player;
        
        if (!_playerRepository.TryAddPlayer(player))
        {
            _logger.LogError($"Player {player.Id} could not be registered");
            await client.DisposeAsync();
            return;
        }
            
        _logger.LogInformation($"Player '{player.Username}' has logged in");
        await _playerRepository.MarkPlayerAsOnlineAsync(player.Id);

        player.Authenticated = true;

        await SendExtraPacketsAsync(client, player);
    }

    private static async Task SendExtraPacketsAsync(INetworkObject client, IPlayer player)
    {
        await client.WriteToStreamAsync(new SecureLoginWriter().GetAllBytes());
        await client.WriteToStreamAsync(new NoobnessLevelWriter(1).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerHomeRoomWriter(player.HomeRoom, player.HomeRoom).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerEffectListWriter(new List<PlayerEffect>()).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerClothingListWriter().GetAllBytes());
        await client.WriteToStreamAsync(new PlayerPermissionsWriter(1, 2, true).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerStatusWriter(true, false, true).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerNavigatorSettingsWriter(player.NavigatorSettings).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerNotificationSettingsWriter(player.Settings.ShowNotifications).GetAllBytes());
        await client.WriteToStreamAsync(new PlayerAchievementScoreWriter(player.AchievementScore).GetAllBytes());
            
        if (player.HasPermission("moderation_tools"))
        {
            await client.WriteToStreamAsync(new ModerationToolsWriter().GetAllBytes());
        }
    }

    private bool ValidateSso(string sso) => 
        !string.IsNullOrEmpty(sso) && sso.Length >= _constants.MinSsoLength;
}