using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Rooms;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.RoomCategories)]
public class RoomCategoriesWriter : AbstractPacketWriter
{
    public required List<RoomCategory> Categories { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Categories.Count);

        foreach (var category in Categories)
        {
            writer.WriteInteger(category.Id);
            writer.WriteString(category.Caption);
            writer.WriteBool(category.IsVisible);
            writer.WriteBool(false); // unknown
            writer.WriteString(category.Caption);
            writer.WriteString(category.Caption.StartsWith("${") ? "" : category.Caption);
            writer.WriteBool(false); // unknown
        }
    }
}