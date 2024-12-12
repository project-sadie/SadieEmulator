using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Inventory;

[PacketId(ServerPacketId.PlayerInventoryAddBot)]
public class PlayerInventoryAddBotWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string Motto { get; init; }
    public required string Gender { get; init; }
    public required string FigureCode { get; init; }
    public required bool OpenInventory { get; init; }
}