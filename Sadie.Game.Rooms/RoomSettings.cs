namespace Sadie.Game.Rooms;

public class RoomSettings : IRoomSettings
{
    public RoomSettings(
        bool walkDiagonal, 
        RoomAccessType accessType,
        string password, 
        int whoCanMute, 
        int whoCanKick, 
        int whoCanBan, bool allowPets, bool canPetsEat, bool hideWalls, bool canUsersOverlap)
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
        CanUsersOverlap = canUsersOverlap;
    }

    public bool CanWalkDiagonal { get; }
    public RoomAccessType AccessType { get; }
    public string Password { get; }
    public int WhoCanMute { get; }
    public int WhoCanKick { get; }
    public int WhoCanBan { get; }
    public bool AllowPets { get; }
    public bool CanPetsEat { get; }
    public bool HideWalls { get; }
    public bool CanUsersOverlap { get; }
}