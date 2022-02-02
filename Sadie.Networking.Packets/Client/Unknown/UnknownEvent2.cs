using Sadie.Networking.Client;

namespace Sadie.Networking.Packets.Client.Unknown;

public class UnknownEvent2 : INetworkPacketEvent
{
    // https://github.com/Domexx/SWF-Source-Clean-MS/blob/6a1d87045dcc8712ad58a9b1329724cbfd4106bc/src/com/sulake/habbo/communication/messages/outgoing/_Str_63/_Str_12290.as
    
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}