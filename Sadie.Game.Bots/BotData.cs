using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Bots;

public record BotData
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string Motto { get; init; }
    public required AvatarGender Gender { get; init; }
    public required string FigureCode { get; init; }
}