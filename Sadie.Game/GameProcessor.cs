using Sadie.Game.Rooms;

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
    
    public async Task ProcessAsync()
    {
        while (true)
        {
            await _roomRepository.RunPeriodicCheckAsync();
            await Task.Delay(500);
        }
    }

    public void Dispose()
    {
        _roomRepository?.Dispose();
        _cts.Cancel();
    }
}