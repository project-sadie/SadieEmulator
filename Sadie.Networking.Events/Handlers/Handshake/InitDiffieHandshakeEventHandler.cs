using Sadie.Networking.Client;
using Sadie.Networking.Encryption;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class InitDiffieHandshakeEventHandler(HabboEncryption habboEncryption) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.InitDiffieHandshake;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new InitDiffieHandshakeWriter
        {
            SingedPrime = habboEncryption.GetRSADiffieHellmanPrimeKey(),
            SignedGenerator = habboEncryption.GetRSADiffieHellmanGeneratorKey()
        });
    }
}