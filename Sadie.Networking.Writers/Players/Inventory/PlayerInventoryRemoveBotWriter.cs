using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryRemoveBot)]
public class PlayerInventoryRemoveBotWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
}