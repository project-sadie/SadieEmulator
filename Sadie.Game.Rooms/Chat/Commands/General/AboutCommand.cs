using System.Diagnostics;
using System.Text;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.General;

public class AboutCommand(
    RoomRepository roomRepository, 
    PlayerRepository playerRepository) : IRoomChatCommand
{
    public string Trigger => "about";

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
        message.AppendLine("");
        
        await user.NetworkObject.WriteToStreamAsync(new PlayerAlertWriter(message.ToString()));
    }
}