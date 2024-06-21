using Sadie.Networking.Client;
using Sadie.Networking.Encryption;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

[PacketId(EventHandlerId.CompleteDiffieHandshake)]
public class CompleteDiffieHandshakeEventHandler(
    HabboEncryption habboEncryption) : INetworkPacketEventHandler
{
    public string? PublicKey { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var sharedKey = habboEncryption.CalculateDiffieHellmanSharedKey(PublicKey);

        await client.WriteToStreamAsync(new CompleteDiffieHandshakeWriter
        {
            PublicKey = habboEncryption.GetRSADiffieHellmanPublicKey(),
            ClientEncryption = true
        });

        client.EnableEncryption(sharedKey);
    }
}