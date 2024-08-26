using Microsoft.EntityFrameworkCore;
using Sadie.API;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Networking.Writers.Players;

namespace CommandsPlugin.Room;

public class UnloadCommand(SadieContext dbContext, IRoomRepository roomRepository) : AbstractRoomChatCommand
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
            await userRepo.TryRemoveAsync(roomUser.Id, true, true);
        }

        if (!roomRepository.TryRemove(user.Room.Id, out var room))
        {
            return;
        }

        await room!.DisposeAsync();
        
        dbContext.Entry(room).State = EntityState.Modified;
        await dbContext.SaveChangesAsync(); 
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_unload"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}