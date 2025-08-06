using Sadie.Networking.Client;
using Sadie.Networking.Encryption;
using Sadie.Networking.Writers.Handshake;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerId.InitDiffieHandshake)]
public class InitDiffieHandshakeEventHandler(HabboEncryption habboEncryption) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new InitDiffieHandshakeWriter
        {
            SingedPrime = habboEncryption.GetRsaDiffieHellmanPrimeKey(),
            SignedGenerator = habboEncryption.GetRsaDiffieHellmanGeneratorKey()
        });
    }
}