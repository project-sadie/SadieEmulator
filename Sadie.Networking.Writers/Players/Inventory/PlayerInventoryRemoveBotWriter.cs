using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryRemoveBot)]
public class PlayerInventoryRemoveBotWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
}