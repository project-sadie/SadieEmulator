using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

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
    public required string StackHeight { get; init; }
    public required int Extra { get; init; }
    public required int ObjectDataKey { get; init; }
    public required Dictionary<string, string> ObjectData { get; init; }
    public required string MetaData { get; init; }
    public required int Expires { get; init; }
    public required int InteractionModes { get; init; }
    public required long OwnerId { get; init; }
    public required string OwnerUsername { get; init; }

    public override void OnConfigureRules()
    {
        Convert<string>(GetType().GetProperty(nameof(PositionZ))!, o => ((double)o).ToString("0.00"));
        Convert<int>(GetType().GetProperty(nameof(InteractionModes))!, o => (int)o > 1 ? 1 : 0);
        
        Override(GetType().GetProperty(nameof(ObjectData))!, writer =>
        {
            if (ObjectDataKey == (int)Enums.Unsorted.ObjectDataKey.LegacyKey)
            {
                writer.WriteString(MetaData);
                return;
            }
            
            writer.WriteInteger(ObjectData.Count);

            foreach (var pair in ObjectData)
            {
                writer.WriteString(pair.Key);
                writer.WriteString(pair.Value);
            }
        });
        
        Override(GetType().GetProperty(nameof(MetaData))!, _ => {});
    }
}