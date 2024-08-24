using Sadie.Enums.Unsorted;

namespace Sadie.Shared.Dtos;

public class FriendData : PlayerFriendshipRequestData
{
    public required string Motto { get; init; }
    public required AvatarGender Gender { get; init; }
}