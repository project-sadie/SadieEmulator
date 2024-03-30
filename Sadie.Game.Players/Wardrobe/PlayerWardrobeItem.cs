using Sadie.Shared.Unsorted.Game.Avatar;

namespace Sadie.Game.Players.Wardrobe;

public class PlayerWardrobeItem(int slotId, string figureCode, AvatarGender gender)
{
    public int SlotId { get; set; } = slotId;
    public string FigureCode { get; set; } = figureCode;
    public AvatarGender Gender { get; set; } = gender;
}