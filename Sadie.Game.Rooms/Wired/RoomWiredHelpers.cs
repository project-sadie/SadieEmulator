using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Furniture;

namespace Sadie.Game.Rooms.Wired;

public static class RoomWiredHelpers
{
    public static int GetTriggerCodeFromInteractionType(string interactionType)
    {
        return interactionType switch
        {
            FurnitureItemInteractionType.WiredTriggerSaysSomething => (int) WiredTriggerCode.AvatarSaysSomething,
            FurnitureItemInteractionType.WiredTriggerEnterRoom => (int) WiredTriggerCode.AvatarEntersRoom,
            FurnitureItemInteractionType.WiredTriggerUserWalksOnFurniture => (int) WiredTriggerCode.AvatarWalksOnFurniture,
            FurnitureItemInteractionType.WiredTriggerUserWalksOffFurniture => (int) WiredTriggerCode.AvatarWalksOffFurniture,
            FurnitureItemInteractionType.WiredTriggerFurnitureStateChanged => (int) WiredTriggerCode.ToggleFurniture,
            FurnitureItemInteractionType.WiredEffectShowMessage => (int) WiredEffectCode.ShowMessage,
            FurnitureItemInteractionType.WiredEffectKickUser => (int) WiredEffectCode.KickUser,
            FurnitureItemInteractionType.WiredEffectTeleportToFurniture => (int) WiredEffectCode.TeleportToFurniture,
            FurnitureItemInteractionType.WiredEffectToggleFurnitureState => 0,
            _ => throw new ArgumentException($"Couldn't match interaction type '{interactionType}' to a trigger layout.")
        };
    }

    public static int GetSelectionCodeFromInteractionType(string interactionType)
    {
        return interactionType switch
        {
            FurnitureItemInteractionType.WiredTriggerUserWalksOnFurniture => 0,
            _ => 1
        };
    }
}