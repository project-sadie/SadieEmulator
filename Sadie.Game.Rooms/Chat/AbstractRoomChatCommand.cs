using Sadie.Game.Rooms.Chat.Commands;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat;

public abstract class AbstractRoomChatCommand : IRoomChatCommand
{
    public abstract string Trigger { get; }
    public abstract string Description { get; }
    public abstract Task ExecuteAsync(IRoomUser user);

    public virtual List<string> PermissionsRequired { get; set; } = [];
}