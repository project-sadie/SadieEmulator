using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Networking.Client;
using Sadie.Networking.Packets.Server.Handshake;
using Sadie.Networking.Packets.Server.Players.Clothing;
using Sadie.Networking.Packets.Server.Players.Effects;
using Sadie.Networking.Packets.Server.Players.Other;
using Sadie.Networking.Packets.Server.Players.Permission;
using Sadie.Networking.Packets.Server.Players.Rooms;
using Sadie.Shared;

namespace Sadie.Networking.Packets.Client.Handshake
{
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
                _logger.LogWarning($"Rejected insecure sso token '{sso}'");
                
                client.Dispose();
                return;
            }
            
            var (foundPlayer, player) = await _playerRepository.TryGetPlayerBySsoAsync(sso);

            if (!foundPlayer || player == null) // put the second check to shut my IDE up about nullable markings.
            {
                _logger.LogWarning($"Failed to find a player with sso token '{sso}'");
                
                client.Dispose();
                return;
            }

            await _playerRepository.ResetSsoTokenForPlayerAsync(player.Id);
            
            client.Player = player;
        }

        private static bool ValidateSso(string sso) => 
            !string.IsNullOrEmpty(sso) && sso.Length >= SadieConstants.HabboSsoMinLength;
    }
}