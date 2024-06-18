using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Server;

public class HotelAlertCommand(PlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "ha";
    public override string Description => "Sends an alert to all online players";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var message = string.Join(" ", parameters);

        if (string.IsNullOrWhiteSpace(message) || message.Length < 5)
        {
            await SendWhisperAsync(user, "Please provide an appropriate message.");
            return;
        }

        var writer = new PlayerAlertWriter
        {
            Message = $"{message}\n\n- {user.Player.Username} at {DateTime.Now:HH:mm}"
        };
        
        await playerRepository.BroadcastDataAsync(writer);
    }
    
    public override List<string> PermissionsRequired{ get; set; } = ["command_hotel_alert"];
}