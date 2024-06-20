using Sadie.API.Game.Rooms.Users;
using Sadie.Game.Players;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.Server;

public class ShutdownCommand(
    IServer server,
    PlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "shutdown";
    public override string Description => "Shuts down the server";

    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        const string shutdownMessage = "The server is about to shut down...";
        
        var writer = new PlayerAlertWriter
        {
            Message = shutdownMessage
        };
        
        await playerRepository.BroadcastDataAsync(writer);

        await Task.Delay(3000);
        await server.DisposeAsync();
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_shutdown"];
}