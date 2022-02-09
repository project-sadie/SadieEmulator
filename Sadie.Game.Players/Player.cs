using Sadie.Game.Players.Avatar;
using Sadie.Game.Players.Navigator;

namespace Sadie.Game.Players;

public class Player : PlayerData, IPlayer
{
    public Player(
        long id, 
        string username, 
        long homeRoom, 
        string figureCode, 
        string motto, 
        PlayerAvatarGender gender, 
        PlayerBalance balance,
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
    }
    
    public bool Authenticated { get; set; }

    public void Dispose()
    {
        Console.WriteLine("Player logged out?");
    }
}