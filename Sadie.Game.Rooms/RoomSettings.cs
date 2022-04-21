namespace Sadie.Game.Rooms;

public class RoomSettings : IRoomSettings
{
    public RoomSettings(
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
    {
        CanWalkDiagonal = walkDiagonal;
        AccessType = accessType;
        Password = password;
        WhoCanMute = whoCanMute;
        WhoCanKick = whoCanKick;
        WhoCanBan = whoCanBan;
        AllowPets = allowPets;
        CanPetsEat = canPetsEat;
        HideWalls = hideWalls;
        WallThickness = wallThickness;
        FloorThickness = floorThickness;
        CanUsersOverlap = canUsersOverlap;
        ChatType = chatType;
        ChatWeight = chatWeight;
        ChatSpeed = chatSpeed;
        ChatDistance = chatDistance;
        ChatProtection = chatProtection;
        TradeOption = tradeOption;
    }

    public bool CanWalkDiagonal { get; }
    public RoomAccessType AccessType { get; set; }
    public string Password { get; set; }
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