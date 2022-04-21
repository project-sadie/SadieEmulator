namespace Sadie.Game.Rooms;

public interface IRoomSettings
{
    bool CanWalkDiagonal { get; }
    RoomAccessType AccessType { get; set; }
    string Password { get; set; }
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