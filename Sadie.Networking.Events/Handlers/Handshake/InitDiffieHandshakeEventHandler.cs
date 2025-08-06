using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Networking.Encryption;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Handshake;

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