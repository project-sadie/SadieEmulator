namespace Sadie.Shared.Dtos;

public class PlayerFriendshipRequestData
{
    public long Id { get; init; }
    public required string Username { get; init; }
    public required string FigureCode { get; init; }
}