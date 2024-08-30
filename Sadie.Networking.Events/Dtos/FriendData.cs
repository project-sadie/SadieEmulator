using Sadie.API;
using Sadie.Enums.Unsorted;

namespace Sadie.Networking.Events.Dtos;

public class FriendData : PlayerFriendshipRequestData, IFriendData
{
    public required string Motto { get; init; }
    public required AvatarGender Gender { get; init; }
}