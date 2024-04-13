using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.General;

public class ShutdownCommand(
    IServer server,
    PlayerRepository playerRepository) : IRoomChatCommand
{
    public string Trigger => "shutdown";
    
    public async Task ExecuteAsync(IRoomUser user)
    {
        if (!playerRepository.TryGetPlayerById(user.Id, out var player) )
        {
            return;
        }

        if (!player.HasPermission("command_shutdown"))
        {
            return;
        }
        
        const string shutdownMessage = "The server is about to shut down...";

        foreach (var p in playerRepository.GetAll())
        {
            await p.NetworkObject.WriteToStreamAsync(new PlayerAlertWriter(shutdownMessage).GetAllBytes());
        }

        await Task.Delay(3000);
        await server.DisposeAsync();
    }
}