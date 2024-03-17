namespace Sadie.Game.Rooms;

public class RoomSettings(
    bool walkDiagonal,
    RoomAccessType accessType,
    string password,
    int whoCanMute,
    int whoCanKick,
    int whoCanBan,
    bool allowPets,
    bool canPetsEat,
    bool hideWalls,
    int wallThickness,
    int floorThickness,
    bool canUsersOverlap,
    int chatType,
    int chatWeight,
    int chatSpeed,
    int chatDistance,
    int chatProtection,
    int tradeOption)
    : IRoomSettings
{
    public bool CanWalkDiagonal { get; } = walkDiagonal;
    public RoomAccessType AccessType { get; set; } = accessType;
    public string Password { get; set; } = password;
    public int WhoCanMute { get; set; } = whoCanMute;
    public int WhoCanKick { get; set; } = whoCanKick;
    public int WhoCanBan { get; set; } = whoCanBan;
    public bool AllowPets { get; set; } = allowPets;
    public bool CanPetsEat { get; set; } = canPetsEat;
    public bool HideWalls { get; set; } = hideWalls;
    public int WallThickness { get; set; } = wallThickness;
    public int FloorThickness { get; set; } = floorThickness;
    public bool CanUsersOverlap { get; set; } = canUsersOverlap;
    public int ChatType { get; set; } = chatType;
    public int ChatWeight { get; set; } = chatWeight;
    public int ChatSpeed { get; set; } = chatSpeed;
    public int ChatDistance { get; set; } = chatDistance;
    public int ChatProtection { get; set; } = chatProtection;
    public int TradeOption { get; set; } = tradeOption;
}