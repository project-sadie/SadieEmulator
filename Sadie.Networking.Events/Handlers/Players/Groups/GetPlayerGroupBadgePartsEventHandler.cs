using Sadie.Networking.Client;
using Sadie.Shared.Attributes;
using Sadie.Networking.Writers.Players.Groups;

namespace Sadie.Networking.Events.Handlers.Players.Groups;

[PacketId(EventHandlerId.GetPlayerGroupBadgeParts)]
public class GetPlayerGroupBadgePartsEventHandler : INetworkPacketEventHandler
{
    public async Task HandleAsync(INetworkClient client)
    {
        var badgeParts = new PlayerGroupBadgePartsWriter
        {
            Bases = [],
            Symbols = [],
            PartColors = [],
            PrimaryColors = [],
            SecondaryColors = []
        }; // TODO: Populate
        
        await client.WriteToStreamAsync(badgeParts);
    }
}