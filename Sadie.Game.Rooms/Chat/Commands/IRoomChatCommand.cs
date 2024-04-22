using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands;

public interface IRoomChatCommand
{
    string Trigger { get; }
    string Description { get; }
    Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters);
    List<string> PermissionsRequired { get; set; }
    bool BypassPermissionCheckIfRoomOwner { get; }
    
}