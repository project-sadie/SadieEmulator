using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Room;

public class UnloadCommand(RoomRepository roomRepository) : AbstractRoomChatCommand, IRoomChatCommand
{
    public override string Trigger => "unload";
    public override string Description => "Unloads all users from your room";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var userRepo = user.Room.UserRepository;
        
        var writer = new PlayerAlertWriter
        {
            Message = "This room has been unloaded."
        };
        
        foreach (var roomUser in user.Room.UserRepository.GetAll())
        {
            await roomUser.Player.NetworkObject!.WriteToStreamAsync(writer);
            await userRepo.TryRemoveAsync(roomUser.Id, true);
        }

        roomRepository.TryUnloadRoom(user.Room.Id, out var room);
        await roomRepository.SaveRoomAsync(room);
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_unload"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}