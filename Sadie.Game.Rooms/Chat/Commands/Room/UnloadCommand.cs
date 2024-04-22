using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Room;

public class UnloadCommand(RoomRepository roomRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "unload";
    public override string Description => "Unloads all users from your room";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var userRepo = user.Room.UserRepository;
        
        foreach (var roomUser in user.Room.UserRepository.GetAll())
        {
            await roomUser.Player.NetworkObject!.WriteToStreamAsync(new PlayerAlertWriter("This room has been unloaded."));
            await userRepo.TryRemoveAsync(roomUser.Id, true);
        }

        roomRepository.TryUnloadRoom(user.Room.Id, out var room);
        await roomRepository.SaveRoomAsync(room);
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_unload"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}