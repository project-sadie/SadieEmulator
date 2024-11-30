using System.Text;
using Sadie.API;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms.Users;

namespace CommandsPlugin.Moderation;

public class UserInfoCommand(IPlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "userinfo";
    public override string Description => "Displays basic info about a user";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var username = parameters.First();
        var target = await playerRepository.GetPlayerByUsernameAsync(username);

        if (target == null)
        {
            await user.SendWhisperAsync("Failed to find the target user.");
            return;
        }
        
        var sb = new StringBuilder();

        sb.AppendLine($"User information for {target.Username}");
        sb.AppendLine("");
        sb.AppendLine($"ID: {target.Id}");
        sb.AppendLine($"Email: {target.Email}");
        sb.AppendLine($"Last Online: {(target.Data.IsOnline ? "now" : target.Data.LastOnline)}");
        sb.AppendLine($"Created: {target.CreatedAt}");

        await user.Player.SendAlertAsync(sb.ToString());
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_user_info"];
}