using Sadie.Networking.Client;
using Sadie.Networking.Encryption;
using Sadie.Networking.Events.Parsers.Handshake;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Handshake;

namespace Sadie.Networking.Events.Handlers.Handshake;

public class CompleteDiffieHandshakeEventHandler(
    CompleteDiffieHandshakeEventParser eventParser,
    HabboEncryption habboEncryption) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.CompleteDiffieHandshake;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var sharedKey = habboEncryption.CalculateDiffieHellmanSharedKey(eventParser.PublicKey!);

        await client.WriteToStreamAsync(new CompleteDiffieHandshakeWriter
        {
            PublicKey = habboEncryption.GetRSADiffieHellmanPublicKey(),
            ClientEncryption = true
        });

        // TODO: Enable encryption for player
    }
}