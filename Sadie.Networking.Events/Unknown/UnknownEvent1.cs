using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Players;

namespace Sadie.Networking.Events.Unknown;

public class UnknownEvent1 : INetworkPacketEvent
{
    // https://github.com/Domexx/SWF-Source-Clean-MS/blob/6a1d87045dcc8712ad58a9b1329724cbfd4106bc/src/com/sulake/habbo/communication/messages/outgoing/inventory/badges/_Str_9250.as
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        await client.WriteToStreamAsync(new PlayerIgnoredUsersWriter().GetAllBytes());
    }
}