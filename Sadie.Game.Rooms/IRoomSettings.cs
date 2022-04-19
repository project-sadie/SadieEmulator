namespace Sadie.Game.Rooms;

public interface IRoomSettings
{
    bool CanWalkDiagonal { get; }
    RoomAccessType AccessType { get; }
    string Password { get; }
    public int WhoCanMute { get; }
    public int WhoCanKick { get; }
    public int WhoCanBan { get; }
    public bool AllowPets { get; }
    public bool CanPetsEat { get; }
    public bool HideWalls { get; }
    public bool CanUsersOverlap { get; }
}