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
        PlayerBalance balance) : base(id, username, homeRoom, figureCode, motto, gender, balance)
    {
    }
    
    public bool Authenticated { get; set; }

    public void Dispose()
    {
        Console.WriteLine("Player logged out?");
    }
}