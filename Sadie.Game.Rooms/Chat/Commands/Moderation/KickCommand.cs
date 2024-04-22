using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Moderation;

public class KickCommand(PlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "kick";
    public override string Description => "Kicks a user out of the current room";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var username = parameters.First();
        var target = playerRepository.GetPlayerLogicByUsername(username);

        if (target == null)
        {
            await SendWhisperAsync(user, "Failed to find target user.");
            return;
        }

        if (!user.Room.UserRepository.TryGetById(target.Id, out var targetUser))
        {
            await SendWhisperAsync(user, "This user isn't in the room.");
            return;
        }

        await targetUser!.Player.NetworkObject!.WriteToStreamAsync(new PlayerAlertWriter("You have been kicked from the room."));
        await user.Room.UserRepository.TryRemoveAsync(targetUser.Id, true);
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_kick"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}