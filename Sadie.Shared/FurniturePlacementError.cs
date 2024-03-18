using System.ComponentModel;

namespace Sadie.Shared;

public enum FurniturePlacementError
{
    [Description("")]
    None,
    
    [Description("${room.error.cant_set_not_owner}")]
    MissingRights,
    
    [Description("${room.error.cant_set_item}")]
    CantSetItem,
    
    [Description("${room.error.max_furniture}")]
    MaxItems,
}