using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryBadges)]
public class PlayerInventoryBadgesWriter : AbstractPacketWriter
{
    public required Dictionary<int, string> Badges { get; init; }
    public required Dictionary<int, string> EquippedBadges { get; init; }
}