using System.Net.Sockets;
using Fleck;

namespace Sadie.Networking.Client;

public interface INetworkClientFactory
{
    INetworkClient CreateClient(Guid guid, IWebSocketConnection tcpClient);
}