namespace Sadie.Game.Players;

public class Player : PlayerData, IPlayer
{
    public Player(long id, string username, long homeRoom, string figureCode, string motto, PlayerAvatarGender gender) : base(id, username, homeRoom, figureCode, motto, gender)
    {
    }

    public void Dispose()
    {
        Console.WriteLine("Player logged out?");
    }
}