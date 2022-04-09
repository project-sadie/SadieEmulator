using Microsoft.Extensions.Logging;
using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public class Player : PlayerData, IPlayer
{
    private readonly ILogger<Player> _logger;

    public Player(
        ILogger<Player> logger,
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
        List<PlayerBadge> badges, 
        List<PlayerFriendshipData> friendships) : 
        
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
            badges, 
            friendships)
    {
        _logger = logger;
    }
    
    public bool Authenticated { get; set; }

    public bool HasPermission(string name) => Permissions.Contains(name);

    public ValueTask DisposeAsync()
    {
        _logger.LogInformation($"Player '{Username}' has logged out");
        return ValueTask.CompletedTask;
    }
}