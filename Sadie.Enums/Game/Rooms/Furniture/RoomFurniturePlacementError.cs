using System.ComponentModel;

namespace Sadie.Enums.Game.Rooms.Furniture;

public enum RoomFurniturePlacementError
{
    [Description("")]
    None,
    
    [Description("${room.error.cant_set_not_owner}")]
    MissingRights,
    
    [Description("${room.error.cant_set_item}")]
    CantSetItem,
    
    [Description("${room.error.max_furniture}")]
    MaxItems,
    
    [Description("${room.error.max_dimmers}")]
    MaxDimmers
}