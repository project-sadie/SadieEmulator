using Sadie.Database.Models.Players;
using Sadie.Enums;
using Sadie.Enums.Unsorted;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Players.Friendships;

public class PlayerFriendshipUpdate
{
    public required int Type { get; init; }
    public required Player? Friend { get; init; }
    public required bool FriendOnline { get; init; }
    public required bool FriendInRoom { get; init; }
    public required PlayerRelationshipType Relation { get; init; }
}