using Sadie.Core.Enums.Game.Players;

namespace Sadie.Db.Models.Players;

public class PlayerWardrobeItem
{
    public int Id { get; init; }
    public int SlotId { get; init; }
    public string? FigureCode { get; init; }
    public PlayerAvatarGender Gender { get; init; }
}