namespace Sadie.Core.Enums.Game.Furniture;

public static class FurnitureItemInteractionType
{
    public const string
        RoomAdsBg = "ads_bg",
        Dimmer = "dimmer",
        Teleport = "teleport",
        Roller = "roller",
        OneWayGate = "onewaygate",
        Gate = "gate",
        VendingMachine = "vending_machine",
        Water = "water",
        WiredTriggerSaysSomething = "wf_trg_says_something",
        WiredTriggerUserWalksOnFurniture = "wf_trg_walks_on_furni",
        WiredTriggerUserWalksOffFurniture = "wf_trg_walks_off_furni",
        WiredTriggerFurnitureStateChanged = "wf_trg_state_changed",
        WiredTriggerEnterRoom = "wf_trg_enter_room",
        WiredEffectShowMessage = "wf_act_show_message",
        WiredEffectKickUser = "wf_act_kick_user",
        WiredEffectTeleportToFurniture = "wf_act_teleport_to",
        WiredEffectMoveRotateFurniture = "wf_act_move_rotate",
        WiredEffectToggleFurnitureState = "wf_act_toggle_state",
        WiredEffectMoveFurnitureToClosestUser = "wf_act_chase",
        WiredEffectMuteTriggerer = "wf_act_mute_triggerer",
        WiredEffectToggleRandomFurnitureState = "wf_xtra_random",
        WiredEffectChangeFurnitureDirection = "wf_act_move_to_dir";
}