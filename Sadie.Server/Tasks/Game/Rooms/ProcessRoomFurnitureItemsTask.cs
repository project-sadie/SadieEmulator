using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture.Processors;
using Sadie.API.Networking;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomFurnitureItemsTask(
    IRoomRepository roomRepository, 
    IEnumerable<IRoomFurnitureItemProcessor> processors) : AbstractTask
{
    public override TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(1000);
    
    public override async Task ExecuteAsync()
    {
        await Parallel.ForEachAsync(roomRepository.GetAllRooms(), BroadcastItemUpdates);
    }

    private async ValueTask BroadcastItemUpdates(IRoomLogic room, CancellationToken ctx)
    {
        var writersToBroadcast = await GetItemUpdatesAsync(room);
        
        foreach (var writer in writersToBroadcast)
        {
            await room.UserRepository.BroadcastDataAsync(writer);
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