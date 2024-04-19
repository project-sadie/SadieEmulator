using Sadie.Shared.Unsorted;

namespace Sadie.Database.Models.Rooms;

public class RoomSettings
{
    public int Id { get; init; }
    public int RoomId { get; init; }
    public bool WalkDiagonal { get; init; }
    public RoomAccessType AccessType { get; set; }
    public string Password { get; set; } = "";
    public int WhoCanMute { get; set; }
    public int WhoCanKick { get; set; }
    public int WhoCanBan { get; set; }
    public bool AllowPets { get; set; }
    public bool CanPetsEat { get; set; }
    public bool HideWalls { get; set; }
    public int WallThickness { get; set; }
    public int FloorThickness { get; set; }
    public bool CanUsersOverlap { get; set; }
    public int ChatType { get; set; }
    public int ChatWeight { get; set; }
    public int ChatSpeed { get; set; }
    public int ChatDistance { get; set; }
    public int ChatProtection { get; set; }
    public int TradeOption { get; set; }
}