using Microsoft.Extensions.Logging;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public class Player : PlayerData, IPlayer
{
    private readonly ILogger<Player> _logger;
    private readonly IPlayerRepository _playerRepository;

    public Player(
        ILogger<Player> logger,
        IPlayerRepository playerRepository,
        int id, 
        string username, 
        DateTime createdAt,
        long homeRoom, 
        string figureCode, 
        string motto, 
        AvatarGender gender, 
        IPlayerBalance balance,
        DateTime lastOnline,
        int respectsReceived, 
        int respectPoints, 
        int respectPointsPet,
        PlayerNavigatorSettings navigatorSettings,
        PlayerSettings settings,
        List<PlayerSavedSearch> savedSearches,
        List<string> permissions,
        long achievementScore,
        List<string> tags,
        List<PlayerBadge> badges) : 
        
        base(
            id, 
            username, 
            createdAt,
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
            permissions,
            achievementScore, 
            tags,
            badges)
    {
        _logger = logger;
        _playerRepository = playerRepository;
    }
    
    public bool Authenticated { get; set; }

    public bool HasPermission(string name) => Permissions.Contains(name);

    public async ValueTask DisposeAsync()
    {
        _playerRepository.TryRemovePlayer(Id);
        await _playerRepository.MarkPlayerAsOfflineAsync(this);
        
        _logger.LogInformation($"Player '{Username}' has logged out");
    }
}