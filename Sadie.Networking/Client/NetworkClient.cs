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
    private readonly TcpClient _tcpClient;
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
        _tcpClient = tcpClient;

        SetClient(this);
    }
        
    public IPlayer? Player { get; set; }
    public RoomUser? RoomUser { get; set; }

    public Task ListenAsync()
    {
        var listeningThread = new Thread(StartListening)
        {
            Name = "Client Listening Thread",
            Priority = ThreadPriority.AboveNormal
        };
        
        listeningThread.Start();
        return Task.CompletedTask;
    }

    public DateTime LastPing { get; set; }


    private bool _disposed;
    
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (RoomUser != null)
        {
            await RoomUser.DisposeAsync();
            RoomUser = null;
        }

        if (Player != null)
        {
            await Player.DisposeAsync();
            Player = null;
        }
        
        if (!_clientRepository.TryRemove(_guid))
        {
            _logger.LogError("Failed to dispose network client.");
        }
        
        _cts.Cancel();

        if (_tcpClient.Connected)
        {
            _tcpClient.GetStream().Close();
            _tcpClient.Close();
        }
    }
}