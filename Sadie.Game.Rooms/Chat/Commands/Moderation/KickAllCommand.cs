using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players.Packets.Writers;

namespace Sadie.Game.Rooms.Chat.Commands.Moderation;

public class KickAllCommand : AbstractRoomChatCommand
{
    public override string Trigger => "kickall";
    public override string Description => "Kicks all users out of the current room";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var userRepo = user.Room.UserRepository;
        
        var alertWriter = new PlayerAlertWriter
        {
            Message = "You have been kicked from the room."
        };
        
        foreach (var roomUser in user.Room.UserRepository.GetAll())
        {
            if (roomUser.Id == user.Id)
            {
                continue;
            }
            
            await roomUser.Player.NetworkObject!.WriteToStreamAsync(alertWriter);
            await userRepo.TryRemoveAsync(roomUser.Id, true);
        }
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_kick_all"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}