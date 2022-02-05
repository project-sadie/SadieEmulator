namespace Sadie.Game.Players;

public class PlayerData : PlayerAvatarData, IPlayerData
{
    protected PlayerData(long id, string username, long homeRoom, string figureCode, string motto, PlayerAvatarGender gender, PlayerBalance balance) : base(figureCode, motto, gender)
    {
        Id = id;
        Username = username;
        HomeRoom = homeRoom;
        Balance = balance;
    }

    public long Id { get; }
    public string Username { get; }
    public long HomeRoom { get; }
    public PlayerBalance Balance { get; }
}