using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.Handlers.Unknown;

public class UnknownEvent3 : INetworkPacketEvent
{
    // https://github.com/Domexx/SWF-Source-Clean-MS/blob/6a1d87045dcc8712ad58a9b1329724cbfd4106bc/src/com/sulake/habbo/communication/messages/outgoing/_Str_499/_Str_10855.as
    
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        return Task.CompletedTask;
    }
}