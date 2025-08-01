using Sadie.Networking.Client;
using Sadie.Networking.Encryption;
using Sadie.Networking.Writers.Handshake;
using Sadie.Shared.Attributes;

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
            PublicKey = habboEncryption.GetRsaDiffieHellmanPublicKey(),
            ClientEncryption = true
        });

        client.EnableEncryption(sharedKey);
    }
}