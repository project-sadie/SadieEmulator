namespace Sadie.Game.Players.Messenger;

public interface IPlayerMessageDao
{
    Task<int> CreateMessageAsync(PlayerMessage message);
}