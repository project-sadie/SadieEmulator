using System.Text;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Moderation;

public class UserInfoCommand(PlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "userinfo";
    public override string Description => "Displays basic info about a user";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var username = parameters.First();
        var target = await playerRepository.GetPlayerByUsernameAsync(username);

        if (target == null)
        {
            await SendWhisperAsync(user, "Failed to find the target user.");
            return;
        }
        
        var sb = new StringBuilder();

        sb.AppendLine($"User information for {target.Username}");
        sb.AppendLine("");
        sb.AppendLine($"ID: {target.Id}");
        sb.AppendLine($"Email: {target.Email}");
        sb.AppendLine($"Last Online: {(target.Data.IsOnline ? "now" : target.Data.LastOnline)}");
        sb.AppendLine($"Created: {target.CreatedAt}");
        
        await user.NetworkObject.WriteToStreamAsync(new PlayerAlertWriter
        {
            Message = sb.ToString()
        });
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_user_info"];
}