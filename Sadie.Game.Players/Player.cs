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
        PlayerSettings settings) : base(id, username, homeRoom, figureCode, motto, gender, balance, lastOnline, respectsReceived, respectPoints, respectPointsPet, navigatorSettings, settings)
    {
    }
    
    public bool Authenticated { get; set; }

    public void Dispose()
    {
        Console.WriteLine("Player logged out?");
    }
}