namespace Sadie.Game.Rooms;

public class RoomSettings : IRoomSettings
{
    public RoomSettings(
        bool walkDiagonal, 
        RoomAccessType accessType,
        string password)
    {
        CanWalkDiagonal = walkDiagonal;
        AccessType = accessType;
        Password = password;
    }

    public bool CanWalkDiagonal { get; }
    public RoomAccessType AccessType { get; }
    public string Password { get; }
}