using System.Net.Sockets;

namespace Sadie.Networking.Client;

public interface INetworkClientFactory
{
    INetworkClient CreateClient(Guid guid, TcpClient tcpClient);
}