namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBotRepository : IAsyncDisposable
{
    ICollection<IRoomBot> GetAll();
    bool TryAdd(IRoomBot bot);
    bool TryGetById(int id, out IRoomBot? bot);
    int Count { get; }
}