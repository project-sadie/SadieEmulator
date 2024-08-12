using Sadie.Enums.Game.Rooms.Users;
using Sadie.Enums.Unsorted;
using Sadie.Shared.Unsorted;

namespace Sadie.Shared.Helpers;

public class RoomHelpers
{
    public static RoomUserEmotion GetEmotionFromMessage(string message)
    {
        List<string> happyEmojis = [":)", ":-)", ":]", ":d"];
        List<string> angryEmojis = [":@", ">:("];
        List<string> shockedEmojis = [":o", ":0", "o.o", "0.0"];
        List<string> sadEmojis = [":(", ":-(", ":["];

        if (happyEmojis.Any(x => message.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            return RoomUserEmotion.Smile;
        }

        if (angryEmojis.Any(x => message.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            return RoomUserEmotion.Angry;
        }

        if (shockedEmojis.Any(x => message.Contains(x, StringComparison.OrdinalIgnoreCase)))
        {
            return RoomUserEmotion.Shocked;
        }

        return sadEmojis.Any(x => message.Contains(x, StringComparison.OrdinalIgnoreCase)) ? 
            RoomUserEmotion.Sad : 
            RoomUserEmotion.None;
    }
}