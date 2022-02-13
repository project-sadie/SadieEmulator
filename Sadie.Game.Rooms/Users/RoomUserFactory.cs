using Sadie.Shared;

namespace Sadie.Game.Rooms.Users;

public static class RoomUserFactory
{
    public static RoomUser Create(long id, HPoint point) => new RoomUser(id, point);
}