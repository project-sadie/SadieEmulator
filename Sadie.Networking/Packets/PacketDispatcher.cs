using System.Threading.Tasks.Dataflow;
using Sadie.API.Interfaces.Networking.Client;

namespace Sadie.Networking.Packets;

public class PacketDispatcher
{
    private readonly INetworkPacketHandler _packetHandler;
    private readonly ActionBlock<(INetworkClient client, INetworkPacket packet)> _block;

    public PacketDispatcher(INetworkPacketHandler packetHandler, int workers = 4)
    {
        _packetHandler = packetHandler;
        _block = new ActionBlock<(INetworkClient client, INetworkPacket packet)>(
            tuple => HandleInternal(tuple.client, tuple.packet),
            new ExecutionDataflowBlockOptions
            {
                MaxDegreeOfParallelism = workers,
                EnsureOrdered = false,
                BoundedCapacity = 5000
            });
    }

    public void Enqueue(INetworkClient client, INetworkPacket packet)
        => _block.Post((client, packet));

    private async Task HandleInternal(INetworkClient client, INetworkPacket packet)
    {
        await _packetHandler.HandleAsync(client, packet);
    }
}