using System.Buffers.Binary;
using System.Net.Sockets;
using Sadie.Game.Players;
using Sadie.Networking.Packets;
using System.Text;

namespace Sadie.Networking.Client
{
    public class NetworkClientWorking : INetworkClient
    {
        private readonly TcpClient _tcpClient;
        private readonly INetworkPacketHandler _packetHandler;
        private readonly NetworkStream _networkStream;

        public NetworkClientWorking(TcpClient tcpClient, INetworkPacketHandler packetHandler)
        {
            _tcpClient = tcpClient;
            _packetHandler = packetHandler;
            _networkStream = _tcpClient.GetStream();
            _buffer = new byte[8000];
        }
        
        public IPlayer? Player { get; set; }

        private byte[] _buffer; 
        
        public async Task ListenAsync()
        {
            _tcpClient.Client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceived, _tcpClient);
        }

        private void OnReceived(IAsyncResult iar)
        {
            var bytesReceived = _tcpClient.Client.EndReceive(iar);
            OnReceived(bytesReceived);
        }

        private void OnReceived(int bytesReceived)
        {
            if (bytesReceived == 0)
            {
                return;
            }
            
            var packet = new byte[bytesReceived];
            Array.Copy(_buffer, packet, bytesReceived);
            
            if (packet[0] == 60)
            {
                var data = Encoding.Default.GetBytes("<?xml version=\"1.0\"?>\r\n" +
                                                     "<!DOCTYPE cross-domain-policy SYSTEM \"/xml/dtds/cross-domain-policy.dtd\">\r\n" +
                                                     "<cross-domain-policy>\r\n" +
                                                     "<allow-access-from domain=\"*\" to-ports=\"1-31111\" />\r\n" +
                                                     "</cross-domain-policy>\x0");
                _tcpClient.GetStream().Write(data, 0, data.Length);
                _tcpClient.Client.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, OnReceived, _tcpClient);
                return;
            }
            
            if (packet.Length < 4)
            {
                return;
            }

            using var reader = new BinaryReader(new MemoryStream(packet));
            var packetLength = BinaryPrimitives.ReadInt32BigEndian(reader.ReadBytes(4));

            if (packetLength is < 0 or > 5120)
            {
                return;
            }
                
            var packetData = reader.ReadBytes(packetLength);

            using var br2 = new BinaryReader(new MemoryStream(packetData));
            var packetId = BinaryPrimitives.ReadInt16BigEndian(br2.ReadBytes(2));
                
            var content = new byte[packetData.Length - 2];
            Buffer.BlockCopy(packetData, 2, content, 0, packetData.Length - 2);

            _packetHandler.HandleAsync(this, new NetworkPacket(packetId, content)).Wait();
        }

        public async Task WriteToStreamAsync(byte[] data)
        {
            await _networkStream.WriteAsync(data, 0, data.Length);
        }
        
        public void Dispose()
        {
            _tcpClient.Close();
        }
    }
}