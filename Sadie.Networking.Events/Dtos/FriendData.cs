using Sadie.API.Game.Players.Friendships;
using Sadie.Enums.Game.Players;

namespace Sadie.Networking.Events.Dtos;

public class FriendData : PlayerFriendshipRequestData, IFriendData
{
    public required string Motto { get; init; }
    public required PlayerAvatarGender Gender { get; init; }
}