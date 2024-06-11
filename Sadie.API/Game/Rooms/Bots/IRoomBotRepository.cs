using Sadie.Database.Models.Players;

namespace Sadie.API.Game.Rooms.Bots;

public interface IRoomBotRepository : IAsyncDisposable
{
    ICollection<PlayerBot> GetAll();
    bool TryAdd(PlayerBot bot);
    bool TryGetById(int id, out PlayerBot? bot);
}