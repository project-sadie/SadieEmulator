namespace Sadie.Game.Players;

public class PlayerData : PlayerAvatarData, IPlayerData
{
    protected PlayerData(
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
        PlayerSettings settings) : base(figureCode, motto, gender)
    {
        Id = id;
        Username = username;
        HomeRoom = homeRoom;
        Balance = balance;
        LastOnline = lastOnline;
        RespectsReceived = respectsReceived;
        RespectPoints = respectPoints;
        RespectPointsPet = respectPointsPet;
        NavigatorSettings = navigatorSettings;
        Settings = settings;
    }

    public long Id { get; }
    public string Username { get; }
    public long HomeRoom { get; }
    public PlayerBalance Balance { get; }
    public DateTime LastOnline { get; }
    public long RespectsReceived { get; }
    public long RespectPoints { get; }
    public long RespectPointsPet { get; }
    public PlayerNavigatorSettings NavigatorSettings { get; }
    public PlayerSettings Settings { get; }
}