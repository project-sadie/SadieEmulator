using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client;

public class NetworkClient : NetworkClientProcessComponent, INetworkClient
{
    private readonly CancellationTokenSource _cts = new();
        
    public NetworkClient(TcpClient tcpClient, INetworkPacketHandler packetHandler, ILogger<NetworkClientProcessComponent> logger) : base(logger, tcpClient, packetHandler)
    {
        SetClient(this);
    }
        
    public IPlayer Player { get; set; } = null!;

    public Task ListenAsync()
    {
        Task.Run(async () =>
        {
            await StartListening(_cts.Token);
        });

        return Task.CompletedTask;
    }

    public new void Dispose()
    {
        _cts.Cancel();
        Player?.Dispose();
    }
}