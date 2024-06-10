using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Database.Models.Rooms;
using Sadie.Database.Models.Server;
using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Players;

public class Player
{
    public int Id { get; init; }
    public string? Username { get; init; }
    public string? Email { get; init; }
    public ICollection<Role> Roles { get; init; } = [];
    public DateTime CreatedAt { get; init; }
    public PlayerData? Data { get; init; }
    public PlayerAvatarData? AvatarData { get; init; }
    public List<PlayerTag> Tags { get; init; } = [];
    public ICollection<PlayerRoomLike> RoomLikes { get; init; } = [];
    [InverseProperty("OriginPlayer")] public ICollection<PlayerRelationship> Relationships { get; init; } = []; //
    public PlayerNavigatorSettings? NavigatorSettings { get; init; }
    public PlayerGameSettings? GameSettings { get; init; }
    public ICollection<PlayerBadge> Badges { get; init; } = [];
    public ICollection<PlayerFurnitureItem> FurnitureItems { get; init; } = [];
    public ICollection<PlayerWardrobeItem> WardrobeItems { get; init; } = [];
    public ICollection<PlayerSubscription> Subscriptions { get; init; } = [];
    [InverseProperty("TargetPlayer")] public ICollection<PlayerRespect> Respects { get; init; } = [];
    public ICollection<PlayerSavedSearch> SavedSearches { get; init; } = [];
    [InverseProperty("OriginPlayer")] public ICollection<PlayerFriendship> OutgoingFriendships { get; init; } = [];
    
    [InverseProperty("TargetPlayer")]
    public ICollection<PlayerFriendship> IncomingFriendships { get; init; } = [];
    [InverseProperty("OriginPlayer")] public ICollection<PlayerMessage> MessagesSent { get; init; } = [];
    [InverseProperty("TargetPlayer")] public ICollection<PlayerMessage> MessagesReceived { get; init; } = [];
    public ICollection<ServerPeriodicCurrencyRewardLog> RewardLogs { get; set; }
    public ICollection<Room> Rooms { get; set; } = [];
    public ICollection<Group> Groups { get; init; } = [];
    
    public int GetAcceptedFriendshipCount()
    {
        return IncomingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted) + 
               OutgoingFriendships.Count(x => x.Status == PlayerFriendshipStatus.Accepted);
    }

    public List<PlayerFriendship> GetMergedFriendships()
    {
        return OutgoingFriendships
            .Union(IncomingFriendships)
            .ToList();
    }

    public bool IsFriendsWith(int targetId)
    {
        return IncomingFriendships.FirstOrDefault(x => x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted) != null 
               || OutgoingFriendships.FirstOrDefault(x => x.TargetPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted) != null;
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
            .FirstOrDefault(x => x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted);
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