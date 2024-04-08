using System.ComponentModel.DataAnnotations.Schema;
using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Players;

public class Player
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string? SsoToken { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedAt { get; set; }
    public PlayerData Data { get; set; }
    public PlayerAvatarData AvatarData { get; set; }
    public List<PlayerTag> Tags { get; set; }
    public List<PlayerRoomLike> RoomLikes { get; set; }
    public List<PlayerRelationship> Relationships { get; set; }
    public PlayerNavigatorSettings NavigatorSettings { get; set; }
    public PlayerGameSettings GameSettings { get; set; }
    public List<PlayerBadge> Badges { get; set; }
    public List<PlayerFurnitureItem> FurnitureItems { get; set; }
    public List<PlayerWardrobeItem> WardrobeItems { get; set; }
    public List<PlayerPermission> Permissions { get; set; }
    public List<PlayerSubscription> Subscriptions { get; set; }
    public List<PlayerRespect> Respects { get; set; }
    public List<PlayerSavedSearch> SavedSearches { get; set; }
    [InverseProperty("OriginPlayer")]
    public List<PlayerFriendship> OutgoingFriendships { get; set; }
    [InverseProperty("TargetPlayer")]
    public List<PlayerFriendship> IncomingFriendships { get; set; }
    [InverseProperty("OriginPlayer")]
    public List<PlayerMessage> MessagesSent { get; set; }
    [InverseProperty("TargetPlayer")]
    public List<PlayerMessage> MessagesReceived { get; set; }
    
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
        var incoming = IncomingFriendships.FirstOrDefault(x =>
            x.OriginPlayerId == targetId && x.Status == PlayerFriendshipStatus.Accepted);

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
        var incoming = IncomingFriendships.FirstOrDefault(x => x.OriginPlayerId == targetId);

        if (incoming != null)
        {
            IncomingFriendships.Remove(incoming);
        }

        var outgoing = OutgoingFriendships.FirstOrDefault(x => x.OriginPlayerId == targetId);
        
        if (outgoing != null)
        {
            OutgoingFriendships.Remove(outgoing);
        }
    }
}