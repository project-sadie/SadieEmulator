using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Rooms;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Furniture;

[PacketId(ServerPacketId.RoomDimmerSettings)]
public class RoomDimmerSettingsWriter : AbstractPacketWriter
{
    public required RoomDimmerSettings DimmerSettings { get; init; }
    public required ICollection<RoomDimmerPreset> DimmerPresets { get; init; }
    
    public override void OnSerialize(INetworkPacketWriter writer) 
    {
        writer.WriteInteger(DimmerPresets.Count);
        writer.WriteInteger(DimmerSettings.PresetId);

        foreach (var preset in DimmerPresets)
        {
            writer.WriteInteger(preset.PresetId);
            writer.WriteInteger(preset.BackgroundOnly ? 2 : 1);
            writer.WriteString(preset.Color);
            writer.WriteInteger(preset.Intensity);
        }
    }
}