using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Enums.Unsorted;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Packets.Writers.Users;

namespace Sadie.Game.Rooms.Chat;

public abstract class AbstractRoomChatCommand : IRoomChatCommand
{
    public abstract string Trigger { get; }
    public abstract string Description { get; }
    public abstract Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters);

    public virtual List<string> PermissionsRequired { get; set; } = [];

    protected static async Task SendWhisperAsync(IRoomUser user, string message)
    {
        var writer = new RoomUserWhisperWriter
        {
            SenderId = user.Id,
            Message = message,
            EmotionId = (int) Shared.Helpers.RoomHelpers.GetEmotionFromMessage(message),
            Bubble = (int)ChatBubble.Default,
            Unknown = 0
        };
        
        await user.NetworkObject.WriteToStreamAsync(writer);
    }

    public virtual bool BypassPermissionCheckIfRoomOwner => false;
}