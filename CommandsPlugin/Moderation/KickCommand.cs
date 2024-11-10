using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;

namespace CommandsPlugin.Moderation;

public class KickCommand(IPlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "kick";
    public override string Description => "Kicks a user out of the current room";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var username = parameters.First();
        var target = playerRepository.GetPlayerLogicByUsername(username);

        if (target == null)
        {
            await user.SendWhisperAsync("Failed to find target user.");
            return;
        }

        if (!user.Room.UserRepository.TryGetById(target.Id, out var targetUser))
        {
            await user.SendWhisperAsync("This user isn't in the room.");
            return;
        }

        await targetUser.Player.SendAlertAsync("You have been kicked from the room.");
        await user.Room.UserRepository.TryRemoveAsync(targetUser.Id, true);
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_kick"];
    public override bool BypassPermissionCheckIfRoomOwner => true;
}