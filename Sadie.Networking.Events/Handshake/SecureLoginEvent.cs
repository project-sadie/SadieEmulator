using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;
using Sadie.Shared;

namespace Sadie.Networking.Events.Handshake;

public class SecureLoginEvent : INetworkPacketEvent
{
    private readonly ILogger<SecureLoginEvent> _logger;
    private readonly IPlayerRepository _playerRepository;
        
    public SecureLoginEvent(ILogger<SecureLoginEvent> logger, IPlayerRepository playerRepository)
    {
        _logger = logger;
        _playerRepository = playerRepository;
    }
        
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var sso = reader.ReadString();

        if (!ValidateSso(sso)) 
        {
            _logger.LogWarning("Rejected an insecure sso token.");
                
            await client.DisposeAsync();
            return;
        }
            
        var (foundPlayer, player) = await _playerRepository.TryGetPlayerBySsoAsync(sso);

        if (!foundPlayer || player == null) // put the second check to shut my IDE up about nullable markings.
        {
            _logger.LogWarning("Failed to resolve player from their provided sso.");
                
            await client.DisposeAsync();
            return;
        }

        await client.WriteToStreamAsync(new SecureLoginWriter().GetAllBytes());
        await client.WriteToStreamAsync(new NoobnessLevelWriter(1).GetAllBytes());
            
        await _playerRepository.ResetSsoTokenForPlayerAsync(player.Id);
            
        client.Player = player;
    }

    private static bool ValidateSso(string sso) => 
        !string.IsNullOrEmpty(sso) && sso.Length >= SadieConstants.PlayerSsoMinLength;
}