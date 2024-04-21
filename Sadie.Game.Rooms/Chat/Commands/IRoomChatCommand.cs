using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands;

public interface IRoomChatCommand
{
    string Trigger { get; }
    string Description { get; }
    Task ExecuteAsync(IRoomUser user);
    List<string> PermissionsRequired { get; set; }
    
}