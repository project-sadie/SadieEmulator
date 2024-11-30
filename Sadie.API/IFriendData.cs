using Sadie.Enums.Unsorted;

namespace Sadie.API;

public interface IFriendData
{
    string Motto { get; init; }
    AvatarGender Gender { get; init; }
    long Id { get; init; }
    string Username { get; init; }
    string FigureCode { get; init; }
}