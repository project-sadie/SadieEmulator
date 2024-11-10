using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;

namespace Sadie.API;

public abstract class AbstractRoomChatCommand : IRoomChatCommand
{
    public abstract string Trigger { get; }
    public abstract string Description { get; }
    public abstract Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters);
    public virtual List<string> PermissionsRequired { get; set; } = [];
    public virtual bool BypassPermissionCheckIfRoomOwner => false;
}