using System.Collections.Concurrent;
using Sadie.API.Game.Rooms.Bots;
using Sadie.Database.Models.Players;

namespace Sadie.Game.Rooms.Bots;

public class RoomBotRepository : IRoomBotRepository
{
    private readonly ConcurrentDictionary<int, PlayerBot> _bots = new();

    public ICollection<PlayerBot> GetAll() => _bots.Values;
    public bool TryAdd(PlayerBot bot) => _bots.TryAdd(bot.Id, bot);
    public bool TryGetById(int id, out PlayerBot? bot) => _bots.TryGetValue(id, out bot);

    public async ValueTask DisposeAsync()
    {
        // TODO release managed resources here
    }
}