using Sadie.Game.Players.Badges;
using Sadie.Game.Players.Balance;
using Sadie.Game.Players.Friendships;
using Sadie.Game.Players.Navigator;
using Sadie.Shared.Game.Avatar;

namespace Sadie.Game.Players;

public class PlayerData : AvatarData, IPlayerData
{
    protected PlayerData(
        int id, 
        string username, 
        DateTime createdAt,
        int homeRoom, 
        string figureCode, 
        string motto, 
        AvatarGender gender, 
        IPlayerBalance balance, 
        DateTime lastOnline, 
        int respectsReceived, 
        int respectPoints, 
        int respectPointsPet, 
        PlayerNavigatorSettings navigatorSettings,
        PlayerSettings settings, 
        List<PlayerSavedSearch> savedSearches,
        List<string> permissions,
        long achievementScore,
        List<string> tags,
        List<PlayerBadge> badges, 
        PlayerFriendshipComponent friendshipComponent,
        int chatBubble, 
        bool allowFriendRequests) : base(username, figureCode, motto, gender, achievementScore, tags, chatBubble)
    {
        Id = id;
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
        Badges = badges;
        FriendshipComponent = friendshipComponent;
        AllowFriendRequests = allowFriendRequests;
    }

    public int Id { get; }
    public DateTime CreatedAt { get; }
    public int HomeRoom { get; }
    public IPlayerBalance Balance { get; }
    public DateTime LastOnline { get; set; }
    public int RespectsReceived { get; set; }
    public int RespectPoints { get; set; }
    public int RespectPointsPet { get; }
    public PlayerNavigatorSettings NavigatorSettings { get; set; }
    public PlayerSettings Settings { get; }
    public List<PlayerSavedSearch> SavedSearches { get; }
    public List<string> Permissions { get; }
    public List<PlayerBadge> Badges { get; }
    public PlayerFriendshipComponent FriendshipComponent { get; }
    public bool AllowFriendRequests { get; }
    public bool HasPermission(string name) => Permissions.Contains(name);

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }
}