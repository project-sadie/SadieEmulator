using Sadie.Game.Rooms.Categories;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class RoomCategoriesWriter : NetworkPacketWriter
{
    public RoomCategoriesWriter(List<RoomCategory> categories)
    {
        WriteShort(ServerPacketId.RoomCategories);
        WriteInt(categories.Count);

        foreach (var category in categories)
        {
            WriteInt(category.Id);
            WriteString(category.Caption);
            WriteBoolean(category.Visible);
            WriteBoolean(false); // unknown
            WriteString(category.Caption);
            WriteString(category.Caption);
            WriteBoolean(false); // unknown
        }
    }
}