namespace Sadie.Game.Players;

public class Player : PlayerData, IPlayer
{
    public Player(long id, long homeRoom) : base(id, homeRoom)
    {
    }

    public void Dispose()
    {
        Console.WriteLine("Player logged out?");
    }
}