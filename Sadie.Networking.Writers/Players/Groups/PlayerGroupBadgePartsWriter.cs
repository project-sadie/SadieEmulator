using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Groups;

[PacketId(ServerPacketId.PlayerGroupBadgeParts)]
public class PlayerGroupBadgePartsWriter : AbstractPacketWriter
{
    public required Dictionary<int, List<string>> Bases { get; set; }
    public required Dictionary<int, List<string>> Symbols { get; set; }
    public required Dictionary<int, string> PartColors { get; set; }
    public required Dictionary<int, string> PrimaryColors { get; set; }
    public required Dictionary<int, string> SecondaryColors { get; set; }
}