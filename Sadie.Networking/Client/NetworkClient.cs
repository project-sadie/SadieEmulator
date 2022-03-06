using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Sadie.Game.Players;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client;

public class NetworkClient : NetworkClientProcessComponent, INetworkClient
{
    private readonly Guid _guid;
    private readonly ILogger<NetworkClient> _logger;
    private readonly INetworkClientRepository _clientRepository;
    private readonly CancellationTokenSource _cts = new();
        
    public NetworkClient(
        Guid guid, 
        ILogger<NetworkClient> logger, 
        INetworkClientRepository clientRepository, 
        TcpClient tcpClient, 
        INetworkPacketHandler packetHandler, 
        ILogger<NetworkClientProcessComponent> baseLogger) : base(baseLogger, tcpClient, packetHandler)
    {
        _guid = guid;
        _logger = logger;
        _clientRepository = clientRepository;
        
        SetClient(this);
    }
        
    public IPlayer? Player { get; set; }
    public RoomUser? RoomUser { get; set; }

    public Task ListenAsync()
    {
        Task.Run(async () =>
        {
            await StartListening(_cts.Token);
        });

        return Task.CompletedTask;
    }

    public DateTime LastPing { get; set; }

    public void Dispose()
    {
        Player?.DisposeAsync();
        Player = null;
        
        RoomUser?.Dispose();
        RoomUser = null;
        
        if (!_clientRepository.TryRemove(_guid))
        {
            _logger.LogError("Failed to dispose network client.");
        }
        
        base.Dispose();
    }
}