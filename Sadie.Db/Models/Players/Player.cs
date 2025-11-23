using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Db.Models.Rooms;
using Sadie.Db.Models.Server;

namespace Sadie.Db.Models.Players;

public class Player
{
    public Player()
    {
    }

    public long Id { get; init; }
    [MaxLength(50)] public required string Username { get; init; }
    [MaxLength(50)] public required string Email { get; init; }
    [MaxLength(60)] public required string Password { get; init; }
    public ICollection<Role> Roles { get; init; } = [];
    public DateTimeOffset CreatedAt { get; init; }
    public PlayerData? Data { get; set; }
    public PlayerAvatarData? AvatarData { get; set; }
    public List<PlayerTag> Tags { get; init; } = [];
    public ICollection<PlayerRoomLike> RoomLikes { get; init; } = [];
    [InverseProperty("OriginPlayer")] public ICollection<PlayerRelationship> Relationships { get; init; } = [];
    
    public PlayerNavigatorSettings? NavigatorSettings { get; set; }
    
    public PlayerGameSettings? GameSettings { get; set; }
    
    public ICollection<PlayerBadge> Badges { get; init; } = [];

    public ICollection<PlayerFurnitureItem> FurnitureItems { get; init; }
    
    public ICollection<PlayerWardrobeItem> WardrobeItems { get; init; } = [];
    public ICollection<PlayerSubscription> Subscriptions { get; init; } = [];
    [InverseProperty("TargetPlayer")] public ICollection<PlayerRespect> Respects { get; init; } = [];
    public ICollection<PlayerSavedSearch> SavedSearches { get; init; } = [];
    
    [InverseProperty("OriginPlayer")]  public ICollection<PlayerFriendship> OutgoingFriendships { get; init; }
    [InverseProperty("TargetPlayer")]  public ICollection<PlayerFriendship> IncomingFriendships { get; init; }
    
    public ICollection<ServerPeriodicCurrencyRewardLog> RewardLogs { get; init; } = [];
    
    public ICollection<Room> Rooms { get; set; }
    public ICollection<PlayerIgnore> Ignores { get; set; }

    public ICollection<Group> Groups { get; init; } = [];
    public ICollection<PlayerBot> Bots { get; init; } = [];
    public ICollection<PlayerRoomVisit> RoomVisits { get; init; } = [];
    public ICollection<PlayerBan> Bans { get; init; } = [];
    public ICollection<PlayerSsoToken> Tokens { get; init; } = [];
}