using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Players.Clothing;
using Sadie.Networking.Packets.Server.Players.Effects;
using Sadie.Networking.Packets.Server.Players.Navigator;
using Sadie.Networking.Packets.Server.Players.Other;
using Sadie.Networking.Packets.Server.Players.Permission;
using Sadie.Networking.Packets.Server.Players.Rooms;

namespace Sadie.Networking.Packets.Client.Players;

public class PlayerActivityEvent : INetworkPacketEvent
{
    private readonly ILogger<PlayerActivityEvent> _logger;
    private readonly IPlayerRepository _playerRepository;

    public PlayerActivityEvent(ILogger<PlayerActivityEvent> logger, IPlayerRepository playerRepository)
    {
        _logger = logger;
        _playerRepository = playerRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (client.Player is {Authenticated: true})
        {
            return;
        }
        
        var type = reader.ReadString();
        var value = reader.ReadString();
        var action = reader.ReadString();
        
        if (type == "Login" && value == "socket" && action == "client.auth_ok")
        {
            await OnLoginAsync(client);
        }
    }

    private async Task OnLoginAsync(INetworkClient networkClient)
    {
        var player = networkClient.Player!;
        
        if (!_playerRepository.TryAddPlayer(player))
        {
            _logger.LogWarning($"Player {player.Id} could not be registered");
            networkClient.Dispose();
            return;
        }
            
        _logger.LogWarning($"Player {player.Id} has logged in");
        await _playerRepository.MarkPlayerAsOnlineAsync(player.Id);

        player.Authenticated = true;
        
        await networkClient.WriteToStreamAsync(new PlayerHomeRoomWriter(player.HomeRoom, 1).GetAllBytes());
        await networkClient.WriteToStreamAsync(new PlayerEffectListWriter().GetAllBytes());
        await networkClient.WriteToStreamAsync(new PlayerClothingListWriter().GetAllBytes());
        await networkClient.WriteToStreamAsync(new PlayerPermissionsWriter(1, 2, true).GetAllBytes());
        await networkClient.WriteToStreamAsync(new PlayerStatusWriter(true, false, true).GetAllBytes());
        await networkClient.WriteToStreamAsync(new PlayerNavigatorSettingsWriter(player.NavigatorSettings).GetAllBytes());
    }
}