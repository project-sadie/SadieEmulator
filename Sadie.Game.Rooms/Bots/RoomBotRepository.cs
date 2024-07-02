using System.Collections.Concurrent;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Serilog;

namespace Sadie.Game.Rooms.Bots;

public class RoomBotRepository(IRoomLogic room) : IRoomBotRepository
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

            if (bots.Count != 0)
            {
                await room.UserRepository.BroadcastDataAsync(new RoomBotStatusWriter
                {
                    Bots = bots
                });

                await room.UserRepository.BroadcastDataAsync(new RoomBotDataWriter
                {
                    Bots = bots
                });
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