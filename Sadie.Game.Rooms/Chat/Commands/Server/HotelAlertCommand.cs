using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Server;

public class HotelAlertCommand(PlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "ha";
    public override string Description => "Sends an alert to all online players";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var message = string.Join(" ", parameters);
        var author = user.Player.Username;

        if (string.IsNullOrWhiteSpace(message) || message.Length < 5)
        {
            await SendWhisperAsync(user, "Please provide an appropriate message.");
            return;
        }
        
        await playerRepository.BroadcastDataAsync(
            new PlayerAlertWriter($"{message}\n\n- {author} at {DateTime.Now:HH:mm}"));
    }
    
    public override List<string> PermissionsRequired{ get; set; } = ["command_hotel_alert"];
}