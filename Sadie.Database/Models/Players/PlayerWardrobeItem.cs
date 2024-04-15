using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Database.Models.Players;

public class PlayerWardrobeItem
{
    public int Id { get; init; }
    public int SlotId { get; init; }
    public string FigureCode { get; init; }
    public AvatarGender Gender { get; init; }
}