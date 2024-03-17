using System.Net.Sockets;
using System.Net.WebSockets;

namespace Sadie.Networking.Client;

public interface INetworkClientFactory
{
    INetworkClient CreateClient(Guid guid, TcpClient tcpClient);
    INetworkClient CreateClient(Guid guid, WebSocket tcpClient);
}