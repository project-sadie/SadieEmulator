using Microsoft.Extensions.Logging;
using Sadie.Game.Players.Avatar;
using Sadie.Game.Players.Navigator;

namespace Sadie.Game.Players;

public class Player : PlayerData, IPlayer
{
    private readonly ILogger<Player> _logger;
    private readonly IPlayerRepository _playerRepository;

    public Player(
        ILogger<Player> logger,
        IPlayerRepository playerRepository,
        long id, 
        string username, 
        long homeRoom, 
        string figureCode, 
        string motto, 
        PlayerAvatarGender gender, 
        IPlayerBalance balance,
        DateTime lastOnline,
        long respectsReceived, 
        long respectPoints, 
        long respectPointsPet,
        PlayerNavigatorSettings navigatorSettings,
        PlayerSettings settings,
        List<PlayerSavedSearch> savedSearches,
        long achievementScore) : 
        
        base(
            id, 
            username, 
            homeRoom, 
            figureCode, 
            motto, 
            gender, 
            balance, 
            lastOnline, 
            respectsReceived, 
            respectPoints, 
            respectPointsPet, 
            navigatorSettings, 
            settings, 
            savedSearches,
            achievementScore)
    {
        _logger = logger;
        _playerRepository = playerRepository;
    }
    
    public bool Authenticated { get; set; }

    public async ValueTask DisposeAsync()
    {
        _playerRepository.TryRemovePlayer(Id);
        await _playerRepository.MarkPlayerAsOfflineAsync(Id);
        
        _logger.LogInformation($"Player '{Username}' has logged out");
    }
}