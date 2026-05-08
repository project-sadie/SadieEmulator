using System.Buffers;
using System.Runtime.InteropServices;
using System.Threading.Tasks.Dataflow;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Packets;

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
        try
        {
            await _packetHandler.HandleAsync(client, packet);
        }
        finally
        {
            if (packet is NetworkPacket p)
            {
                if (MemoryMarshal.TryGetArray(p.Data, out var segment))
                {
                    ArrayPool<byte>.Shared.Return(segment.Array!);
                }
            }
        }
    }
}