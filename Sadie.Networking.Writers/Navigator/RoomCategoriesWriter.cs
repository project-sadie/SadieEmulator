using Sadie.Game.Rooms.Categories;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class RoomCategoriesWriter : NetworkPacketWriter
{
    public RoomCategoriesWriter(List<RoomCategory> categories)
    {
        WriteShort(ServerPacketId.RoomCategories);
        WriteInteger(categories.Count);

        foreach (var category in categories)
        {
            WriteInteger(category.Id);
            WriteString(category.Caption);
            WriteBool(category.Visible);
            WriteBool(false); // unknown
            WriteString(category.Caption);
            WriteString(category.Caption.StartsWith("${") ? "" : category.Caption);
            WriteBool(false); // unknown
        }
    }
}