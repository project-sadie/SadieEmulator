using System.ComponentModel;

namespace Sadie.Enums.Unsorted;

public enum NotificationType
{
    [Description("furni_placement_error")]
    FurniturePlacementError,
    
    [Description("floorplan_editor.error")]
    FloorPlanEditor
}