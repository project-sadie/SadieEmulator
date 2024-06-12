using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database;
using Sadie.Game.Rooms.Furniture;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Room;

public class StateCommand(SadieContext dbContext, RoomRepository roomRepository) : AbstractRoomChatCommand, IRoomChatCommand
{
    public override string Trigger => "state";
    public override string Description => "Unloads all users from your room";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        foreach (var item in user.Room.FurnitureItems)
        {
            item.MetaData = parameters.First();
            await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(user.Room, item);
        }
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_unload"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}