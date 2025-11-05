namespace Sadie.Core.Enums.Miscellaneous;

public enum GenericErrorCode
{
    AuthenticationFailed = -3,
    ServerConnectionFailed = -400,
    
    NavigatorInvalidPassword = -100002,
    NavigatorRoomRequiresVip = 4009,
    NavigatorRoomInvalidName = 4010,
    NavigatorRoomCannotPermBan = 4011,
    NavigatorRoomInMaintenance = 4013,
    
    RoomKickedByOwner = 4008,
    StripLockedForTrading = -13001,
}