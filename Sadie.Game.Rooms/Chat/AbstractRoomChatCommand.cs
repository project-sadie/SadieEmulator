using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted.Game;

namespace Sadie.Game.Rooms.Chat;

public abstract class AbstractRoomChatCommand : IRoomChatCommand
{
    public abstract string Trigger { get; }
    public abstract string Description { get; }
    public abstract Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters);

    public virtual List<string> PermissionsRequired { get; set; } = [];

    protected static async Task SendWhisperAsync(IRoomUser user, string message)
    {
        await user.NetworkObject.WriteToStreamAsync(new RoomUserWhisperWriter(
            user.Id,
            message,
            0,
            (int) ChatBubble.Default));

    }

    public virtual bool BypassPermissionCheckIfRoomOwner => false;
}