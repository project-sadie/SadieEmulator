using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Networking.Client;

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
        // TODO: Stop this from executing multiple times for one connection.
        
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
        var player = networkClient.Player;
        
        if (player == null || !_playerRepository.TryAddPlayer(player))
        {
            _logger.LogWarning($"Player {player.Id} could not be registered");
            networkClient.Dispose();
            return;
        }
            
        _logger.LogWarning($"Player {player.Id} has logged in");
        await _playerRepository.MarkPlayerAsOnlineAsync(player.Id);
    }
}