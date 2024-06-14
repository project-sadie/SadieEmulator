using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomFloorFurnitureItemPlaced)]
public class RoomFloorItemPlacedWriter : AbstractPacketWriter
{
    public required long Id { get; init; }
    public required int AssetId { get; init; }
    public required int PositionX { get; init; }
    public required int PositionY { get; init; }
    public required int Direction { get; init; }
    public required double PositionZ { get; init; }
    public required int StackHeight { get; init; }
    public required int Extra { get; init; }
    public required int ObjectDataKey { get; init; }
    public required string MetaData { get; init; }
    public required int Expires { get; init; }
    public required int InteractionModes { get; init; }
    public required long OwnerId { get; init; }
    public required string OwnerUsername { get; init; }

    public override void OnConfigureRules()
    {
        Convert<string>(GetType().GetProperty(nameof(PositionZ))!, o => ((double)o).ToString("0.00"));
        Convert<string>(GetType().GetProperty(nameof(StackHeight))!, o => o.ToString());
        Convert<int>(GetType().GetProperty(nameof(InteractionModes))!, o => (int)o > 1 ? 1 : 0);
    }
}