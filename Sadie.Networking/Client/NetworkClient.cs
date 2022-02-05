using System.Net.Sockets;
using Sadie.Game.Players;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Client
{
    public class NetworkClient : NetworkClientProcessComponent, INetworkClient
    {
        public NetworkClient(TcpClient tcpClient, INetworkPacketHandler packetHandler) : base(tcpClient, packetHandler)
        {
            SetClient(this);
        }
        
        public IPlayer? Player { get; set; }

        public async Task ListenAsync()
        {
            Task.Run(async () =>
            {
                await StartListening();
            });
        }

        public new void Dispose()
        {
            Player?.Dispose();
        }
    }
}