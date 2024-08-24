using Sadie.Enums.Game.Players;

namespace Sadie.Shared.Dtos;

public class PlayerFriendshipUpdate
{
    public required int Type { get; init; }
    public required FriendData? Friend { get; init; }
    public required bool FriendOnline { get; init; }
    public required bool FriendInRoom { get; init; }
    public required PlayerRelationshipType Relation { get; init; }
}