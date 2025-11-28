using Sadie.API.Interfaces.Game.Players.Friendships;
using Sadie.Core.Enums.Game.Players;

namespace Sadie.Networking.Events.Dtos;

public class FriendData : PlayerFriendshipRequestData, IFriendData
{
    public required string Motto { get; init; }
    public required PlayerAvatarGender Gender { get; init; }
}