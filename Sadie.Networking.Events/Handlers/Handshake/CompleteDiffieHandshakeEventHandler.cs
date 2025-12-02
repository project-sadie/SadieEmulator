using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Networking.Encryption;
using Sadie.Networking.Packets.Writers.Handshake;

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