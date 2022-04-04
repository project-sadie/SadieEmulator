using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public class PlayerData : AvatarData, IPlayerData
{
    protected PlayerData(
        long id, 
        string username, 
        DateTime createdAt,
        long homeRoom, 
        string figureCode, 
        string motto, 
        AvatarGender gender, 
        IPlayerBalance balance, 
        DateTime lastOnline, 
        long respectsReceived, 
        long respectPoints, 
        long respectPointsPet, 
        PlayerNavigatorSettings navigatorSettings,
        PlayerSettings settings, 
        List<PlayerSavedSearch> savedSearches,
        List<string> permissions,
        long achievementScore,
        List<string> tags) : base(username, figureCode, motto, gender, achievementScore, tags)
    {
        Id = id;
        Username = username;
        CreatedAt = createdAt;
        HomeRoom = homeRoom;
        Balance = balance;
        LastOnline = lastOnline;
        RespectsReceived = respectsReceived;
        RespectPoints = respectPoints;
        RespectPointsPet = respectPointsPet;
        NavigatorSettings = navigatorSettings;
        Settings = settings;
        SavedSearches = savedSearches;
        Permissions = permissions;
    }

    public long Id { get; }
    public string Username { get; }
    public DateTime CreatedAt { get; }
    public long HomeRoom { get; }
    public IPlayerBalance Balance { get; }
    public DateTime LastOnline { get; }
    public long RespectsReceived { get; }
    public long RespectPoints { get; }
    public long RespectPointsPet { get; }
    public PlayerNavigatorSettings NavigatorSettings { get; set; }
    public PlayerSettings Settings { get; }
    public List<PlayerSavedSearch> SavedSearches { get; }
    public List<string> Permissions { get; }
}