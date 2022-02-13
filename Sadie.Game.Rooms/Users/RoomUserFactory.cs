using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public static class RoomUserFactory
{
    public static RoomUser Create(long id, HPoint point, HDirection direction) => new RoomUser(id, point, direction);
}