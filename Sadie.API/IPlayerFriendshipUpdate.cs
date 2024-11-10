using Sadie.Enums.Game.Players;

namespace Sadie.API;

public interface IPlayerFriendshipUpdate
{
    int Type { get; init; }
    IFriendData? Friend { get; init; }
    bool FriendOnline { get; init; }
    bool FriendInRoom { get; init; }
    PlayerRelationshipType Relation { get; init; }
}