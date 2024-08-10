using System.ComponentModel;

namespace Sadie.Enums.Unsorted;

public enum FurnitureItemType
{
    [Description("s")]
    Floor,
    
    [Description("i")]
    Wall,
    
    [Description("b")]
    Badge,
    
    [Description("e")]
    Effect,
    
    [Description("r")]
    Bot,
    
    [Description("p")]
    Pet,
    
    [Description("h")]
    Club
}