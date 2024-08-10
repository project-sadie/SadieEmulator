using System.Collections.Concurrent;
using Sadie.API.Game.Rooms.Bots;
using Serilog;

namespace Sadie.Game.Rooms.Bots;

public class RoomBotRepository() : IRoomBotRepository
{
    private readonly ConcurrentDictionary<int, IRoomBot> _bots = new();

    public ICollection<IRoomBot> GetAll() => _bots.Values;
    public bool TryAdd(IRoomBot bot) => _bots.TryAdd(bot.Bot.Id, bot);
    public bool TryGetById(int id, out IRoomBot? bot) => _bots.TryGetValue(id, out bot);
    public int Count => _bots.Count;
    
    public async Task RunPeriodicCheckAsync()
    {
        try
        {
            var bots = _bots.Values;

            foreach (var bot in bots)
            {
                await bot.RunPeriodicCheckAsync();
            }
        }
        catch (Exception e)
        {
            Log.Logger.Error(e.ToString());
        }
    }

    public async ValueTask DisposeAsync()
    {
    }
}