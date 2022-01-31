using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Client.Tracking
{
    public class PerformanceLogPacket : INetworkPacketEvent
    {
        public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
        {
            return Task.CompletedTask;
        }
    }
}