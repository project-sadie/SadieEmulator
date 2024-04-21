using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Shared;
using Sadie.Shared.Unsorted;

namespace Sadie.Game.Rooms.Chat.Commands.General;

public class ShutdownCommand(
    IServer server,
    PlayerRepository playerRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "shutdown";
    public override string Description => "Shuts down the server";

    public override async Task ExecuteAsync(IRoomUser user)
    {
        if (!playerRepository.TryGetPlayerById(user.Id, out var player) )
        {
            return;
        }

        const string shutdownMessage = "The server is about to shut down...";

        foreach (var p in playerRepository.GetAll())
        {
            await p.NetworkObject.WriteToStreamAsync(new PlayerAlertWriter(shutdownMessage));
        }

        await Task.Delay(3000);
        await server.DisposeAsync();
    }

    public override List<string> PermissionsRequired { get; set; } = ["command_shutdown"];
}