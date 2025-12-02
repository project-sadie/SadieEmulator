using System.Threading.Channels;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Handlers;

public class PacketWorkerPool
{
    private readonly Channel<(INetworkClient client, INetworkPacket packet)> _channel;
    private readonly INetworkPacketHandler _packetHandler;

    public PacketWorkerPool(INetworkPacketHandler packetHandler)
    {
        _packetHandler = packetHandler;

        _channel = Channel.CreateUnbounded<(INetworkClient, INetworkPacket)>(
            new UnboundedChannelOptions
            {
                SingleReader = false,
                SingleWriter = false
            });

        var workerCount = Environment.ProcessorCount;

        for (var i = 0; i < workerCount; i++)
        {
            Task.Run(WorkerLoop);
        }
    }

    public void Enqueue(INetworkClient client, INetworkPacket packet)
    {
        _channel.Writer.TryWrite((client, packet));
    }

    private async Task WorkerLoop()
    {
        try
        {
            await foreach (var (client, packet) in _channel.Reader.ReadAllAsync())
            {
                try
                {
                    await _packetHandler.HandleAsync(client, packet)
                        .ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Worker crashed: " + ex);
            _ = Task.Run(WorkerLoop);
        }
    }
}