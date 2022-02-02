namespace Sadie.Game.Players;

public class PlayerData : PlayerAvatarData, IPlayerData
{
    protected PlayerData(long id, string username, long homeRoom, string figureCode, string motto, PlayerAvatarGender gender) : base(figureCode, motto, gender)
    {
        Id = id;
        Username = username;
        HomeRoom = homeRoom;
    }

    public long Id { get; }
    public string Username { get; }
    public long HomeRoom { get; }
}