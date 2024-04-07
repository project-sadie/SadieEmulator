using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Database.Models.Players;

public class PlayerWardrobeItem
{
    public int Id { get; set; }
    public int SlotId { get; set; }
    public string FigureCode { get; set; }
    public AvatarGender Gender { get; set; }
}