namespace Sadie.Game.Rooms.Chat.Commands.General;

public class AboutCommand : IRoomChatCommand
{
    public Task ExecuteAsync()
    {
        return Task.CompletedTask;
    }
}