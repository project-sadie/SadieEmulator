using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture.Processors;
using Sadie.API.Interfaces.Networking;
using Sadie.API.Interfaces.Server.Tasks;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomFurnitureItemsTask(
    IRoomRepository roomRepository, 
    IEnumerable<IRoomFurnitureItemProcessor> processors) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(1000);
    public long LastExecutedTicks { get; set; }
    
    public async Task ExecuteAsync()
    {
        await Parallel.ForEachAsync(roomRepository.GetAllRooms(), BroadcastItemUpdates);
    }

    private async ValueTask BroadcastItemUpdates(IRoomLogic room, CancellationToken ctx)
    {
        var writersToBroadcast = await GetItemUpdatesAsync(room);
        
        foreach (var writer in writersToBroadcast)
        {
            await room.BroadcastDataAsync(writer);
        }
    }

    private async Task<IEnumerable<AbstractPacketWriter>> GetItemUpdatesAsync(IRoomLogic room)
    {
        var writers = new List<AbstractPacketWriter>();

        foreach (var processor in processors)
        {
            writers.AddRange(await processor.GetUpdatesForRoomAsync(room));
        }
        
        return writers;
    }
}