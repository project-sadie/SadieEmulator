using Sadie.Networking.Client;
using Sadie.Networking.Packets;

namespace Sadie.Networking.Events.GameCenter;

public class UnknownEvent4 : INetworkPacketEvent
{
    public Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        // https://github.com/Domexx/SWF-Source-Clean-MS/blob/6a1d87045dcc8712ad58a9b1329724cbfd4106bc/src/com/sulake/habbo/game/IncomingMessages.as#L119
        
        return Task.CompletedTask;
    }
}