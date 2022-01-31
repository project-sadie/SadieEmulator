namespace Sadie.Game.Players;

public class PlayerData : IPlayerData
{
    protected PlayerData(long id, long homeRoom)
    {
        Id = id;
        HomeRoom = homeRoom;
    }

    public long Id { get; }
    public long HomeRoom { get; }
}