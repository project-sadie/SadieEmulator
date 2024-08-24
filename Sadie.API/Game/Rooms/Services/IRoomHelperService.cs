using Sadie.Enums.Game.Rooms.Users;

namespace Sadie.API.Game.Rooms.Services;

public interface IRoomHelperService
{
    RoomUserEmotion GetEmotionFromMessage(string message);
}