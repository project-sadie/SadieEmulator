using Sadie.Networking.Client;
using Sadie.Networking.Encryption;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerIds.InitDiffieHandshake)]
public class InitDiffieHandshakeEventHandler(HabboEncryption habboEncryption) : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        await client.WriteToStreamAsync(new InitDiffieHandshakeWriter
        {
            SingedPrime = habboEncryption.GetRSADiffieHellmanPrimeKey(),
            SignedGenerator = habboEncryption.GetRSADiffieHellmanGeneratorKey()
        });
    }
}