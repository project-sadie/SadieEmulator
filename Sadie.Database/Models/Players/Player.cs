using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Server;
using Sadie.Enums.Game.Players;

namespace Sadie.Database.Models.Players;

public class Player : IPlayer
{
    private readonly ILazyLoader? _lazyLoader;

    public Player()
    {
    }
    
    public Player(ILazyLoader? lazyLoader)
    {
        _lazyLoader = lazyLoader;
    }

    private PlayerNavigatorSettings? _navigatorSettings;
    private PlayerGameSettings? _gameSettings;
    private ICollection<PlayerFriendship>? _outgoingFriendships = [];
    private ICollection<PlayerFriendship> _incomingFriendships = [];
    private ICollection<PlayerFurnitureItem>? _furnitureItems = [];

    public int Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public ICollection<Role> Roles { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public required PlayerData Data { get; init; }
    public PlayerAvatarData? AvatarData { get; init; }
    public List<PlayerTag> Tags { get; init; } = [];
    public ICollection<PlayerRoomLike> RoomLikes { get; init; } = [];
    [InverseProperty("OriginPlayer")] public ICollection<PlayerRelationship> Relationships { get; init; } = [];
    
    public PlayerNavigatorSettings? NavigatorSettings
    {
        get => _lazyLoader.Load(this, ref _navigatorSettings);
        init => _navigatorSettings = value;
    }
    
    public PlayerGameSettings? GameSettings
    {
        get => _lazyLoader.Load(this, ref _gameSettings);
        init => _gameSettings = value;
    }
    
    public ICollection<PlayerBadge> Badges { get; init; } = [];

    public ICollection<PlayerFurnitureItem>? FurnitureItems
    {
        get => _lazyLoader.Load(this, ref _furnitureItems);
        init => _furnitureItems = value;
    }
    
    public ICollection<PlayerWardrobeItem> WardrobeItems { get; init; } = [];
    public ICollection<PlayerSubscription> Subscriptions { get; init; } = [];
    [InverseProperty("TargetPlayer")] public ICollection<PlayerRespect> Respects { get; init; } = [];
    public ICollection<PlayerSavedSearch> SavedSearches { get; init; } = [];
    
    [InverseProperty("OriginPlayer")]  public ICollection<PlayerFriendship> OutgoingFriendships
    {
        get => _lazyLoader.Load(this, ref _outgoingFriendships);
        init => _outgoingFriendships = value;
    }
    
    [InverseProperty("TargetPlayer")]  public ICollection<PlayerFriendship> IncomingFriendships
    {
        get => _lazyLoader.Load(this, ref _incomingFriendships);
        init => _incomingFriendships = value;
    }
    
    public ICollection<ServerPeriodicCurrencyRewardLog> RewardLogs { get; init; } = [];
    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<Group> Groups { get; init; } = [];
    public ICollection<PlayerBot> Bots { get; init; } = [];
    public ICollection<PlayerRoomVisit> RoomVisits { get; init; } = [];
    
    public int GetAcceptedFriendshipCount()
    {
        return IncomingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted) + 
               OutgoingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted);
    }

    public List<PlayerFriendship> GetMergedFriendships()
    {
        return OutgoingFriendships
            .Concat(IncomingFriendships)
            .ToList();
    }

    public bool IsFriendsWith(int targetId)
    {
        return IncomingFriendships.FirstOrDefault(x =>
                   x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted) !=
               null 
               ||
               OutgoingFriendships.FirstOrDefault(x =>
                   x.TargetPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted) !=
               null;
    }

    public PlayerFriendship? TryGetAcceptedFriendshipFor(int targetId)
    {
        var incoming = IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted);

        if (incoming != null)
        {
            return incoming;
        }
        
        return OutgoingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted);
    }

    public PlayerFriendship? TryGetFriendshipFor(int targetId)
    {
        var incoming = IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId);

        if (incoming != null)
        {
            return incoming;
        }
        
        return OutgoingFriendships
            .FirstOrDefault(x => x.TargetPlayerId == targetId);
    }

    public void DeleteFriendshipFor(int targetId)
    {
        var incoming = IncomingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId);

        if (incoming != null)
        {
            IncomingFriendships.Remove(incoming);
        }

        var outgoing = OutgoingFriendships
            .FirstOrDefault(x => x.OriginPlayerId == targetId);
        
        if (outgoing != null)
        {
            OutgoingFriendships.Remove(outgoing);
        }
    }

    public bool HasPermission(string name)
    {
        return Roles.Any(r => r.Permissions.Any(x => x.Name == name));
    }
}