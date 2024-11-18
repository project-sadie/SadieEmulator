using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;

namespace Sadie.Game.Rooms.Chat.Commands.User;

public class SitCommand(IPlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "sit";
    public override string Description => "Makes your avatar sit down";
    
    public override Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        if (user.StatusMap.ContainsKey(RoomUserStatus.Sit) ||
            (int) user.Direction % 2 != 0)
        {
            return Task.CompletedTask;
        }
        
        user.AddStatus(RoomUserStatus.Sit, "0.5");
        return Task.CompletedTask;
    }
}