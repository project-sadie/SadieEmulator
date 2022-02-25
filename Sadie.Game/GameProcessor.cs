using Sadie.Game.Rooms;
using Sadie.Shared.Utilities;

namespace Sadie.Game;

public class GameProcessor : IGameProcessor
{
    private readonly IRoomRepository _roomRepository;

    public GameProcessor(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task ProcessAsync()
    {
        await TimerUtilities.RunPeriodically(TimeSpan.FromMilliseconds(500), _roomRepository.RunPeriodicCheckAsync);
    }
}