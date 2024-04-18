using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Players;

public class Player
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string? SsoToken { get; set; }
    public ICollection<Role> Roles { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public PlayerData Data { get; set; }
    public PlayerAvatarData AvatarData { get; set; }
    public ICollection<PlayerTag> Tags { get; set; }
    public ICollection<PlayerRoomLike> RoomLikes { get; set; }
    [InverseProperty("OriginPlayer")] public ICollection<PlayerRelationship> Relationships { get; set; } = [];
    public PlayerNavigatorSettings NavigatorSettings { get; set; }
    public PlayerGameSettings GameSettings { get; set; }
    public ICollection<PlayerBadge> Badges { get; set; } = [];
    public ICollection<PlayerFurnitureItem> FurnitureItems { get; set; } = [];
    public ICollection<PlayerWardrobeItem> WardrobeItems { get; set; } = [];
    public ICollection<PlayerSubscription> Subscriptions { get; set; } = [];
    [InverseProperty("TargetPlayer")] public ICollection<PlayerRespect> Respects { get; set; } = [];
    public ICollection<PlayerSavedSearch> SavedSearches { get; set; } = [];
    [InverseProperty("OriginPlayer")] public ICollection<PlayerFriendship> OutgoingFriendships { get; set; } = [];
    
    [InverseProperty("TargetPlayer")]
    public ICollection<PlayerFriendship> IncomingFriendships { get; set; } = [];
    [InverseProperty("OriginPlayer")] public ICollection<PlayerMessage> MessagesSent { get; set; } = [];
    [InverseProperty("TargetPlayer")] public ICollection<PlayerMessage> MessagesReceived { get; set; } = [];
    
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