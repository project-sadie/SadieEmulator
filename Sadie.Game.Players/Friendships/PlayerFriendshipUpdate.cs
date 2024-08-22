using Sadie.Database.Models.Players;
using Sadie.Enums.Game.Players;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipUpdate
{
    public required int Type { get; init; }
    public required IPlayer? Friend { get; init; }
    public required bool FriendOnline { get; init; }
    public required bool FriendInRoom { get; init; }
    public required PlayerRelationshipType Relation { get; init; }
}