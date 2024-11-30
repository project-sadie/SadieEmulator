using System.Text;
using Sadie.API;
using Sadie.API.Game.Rooms.Chat.Commands;
using Sadie.API.Game.Rooms.Users;

namespace CommandsPlugin.Generic;

public class ListCommandsCommand(IRoomChatCommandRepository chatCommandRepository) : AbstractRoomChatCommand
{
    public override string Trigger => "commands";
    public override string Description => "View all available commands";
    
    public override async Task ExecuteAsync(IRoomUser user, IEnumerable<string> parameters)
    {
        var commands = chatCommandRepository.GetRegisteredCommands();
        var sb = new StringBuilder();

        sb.AppendLine($"{commands.Count} commands");
        
        foreach (var command in commands)
        {
            sb.AppendLine($":{command.Trigger} - {command.Description}");
        }
        
        await user.Player.SendAlertAsync(sb.ToString());
    }
}