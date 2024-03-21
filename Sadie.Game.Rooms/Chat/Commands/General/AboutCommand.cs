using System.Diagnostics;
using System.Text;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Writers.Players;
using Sadie.Shared;

namespace Sadie.Game.Rooms.Chat.Commands.General;

public class AboutCommand(
    IRoomRepository roomRepository, 
    IPlayerRepository playerRepository) : IRoomChatCommand
{
    public async Task ExecuteAsync(IRoomUser user)
    {
        var version = GlobalState.Version;
        var message = new StringBuilder();
        var memoryMb = Process.GetCurrentProcess().PrivateMemorySize64 / (1024 * 1024);

        message.AppendLine($"Sadie {version}");
        message.AppendLine("");
        message.AppendLine($"Players Online: {playerRepository.Count()}");
        message.AppendLine($"Rooms Loaded: {roomRepository.Count}");
        message.AppendLine($"Memory Used: {memoryMb} MB");
        message.AppendLine("");
        message.AppendLine("Credits:");
        message.AppendLine("Habtard - Solo Developer");
        message.AppendLine("Lucas - Testing & Support");
        message.AppendLine("");
        
        await user.NetworkObject.WriteToStreamAsync(new PlayerAlertWriter(message.ToString()).GetAllBytes());
    }
}