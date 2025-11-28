using Sadie.API.Interfaces.Game.Players.Friendships;
using Sadie.Core.Enums.Game.Players;

namespace Sadie.Networking.Events.Dtos;

public class PlayerFriendshipUpdate : IPlayerFriendshipUpdate
{
    public required int Type { get; init; }
    public required IFriendData? Friend { get; init; }
    public required bool FriendOnline { get; init; }
    public required bool FriendInRoom { get; init; }
    public required PlayerRelationshipType Relation { get; init; }
}