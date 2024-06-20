using System.Diagnostics;
using System.Text;
using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
using Sadie.Shared;

namespace Sadie.Game.Rooms.Chat.Commands.Server;

public class AboutCommand(
    RoomRepository roomRepository, 
    PlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "about";
    public override string Description => "Provides information about the server";

    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var version = GlobalState.Version;
        var message = new StringBuilder();
        var memoryMb = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);

        message.AppendLine($"Sadie Emulator {version}");
        message.AppendLine("");
        message.AppendLine($"Players Online: {playerRepository.Count()}");
        message.AppendLine($"Rooms Loaded: {roomRepository.Count}");
        message.AppendLine($"Memory Used: {memoryMb} MB");
        message.AppendLine("");
        message.AppendLine("Credits:");
        message.AppendLine("habtard - Developer");
        message.AppendLine("Lucas - Graphics");
        message.AppendLine("React - Testing");
        message.AppendLine("");

        await user.Player.SendAlertAsync(message.ToString());
    }
}