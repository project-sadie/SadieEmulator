using Sadie.Game.Rooms;
using Sadie.Shared.Utilities;

namespace Sadie.Game;

public class GameProcessor : IGameProcessor
{
    private readonly IRoomRepository _roomRepository;
    private readonly CancellationTokenSource _cts;

    public GameProcessor(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
        _cts = new CancellationTokenSource();
    }

    public Task Boot()
    {
        ProcessAsync();
        return Task.CompletedTask;
    }
    
    public async Task ProcessAsync()
    {
        await TimerUtilities.RunPeriodically(TimeSpan.FromMilliseconds(500), _roomRepository.RunPeriodicCheckAsync, _cts.Token);
    }

    public void Dispose()
    {
        _roomRepository?.Dispose();
        _cts.Cancel();
    }
}