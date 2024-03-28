using Sadie.Game.Rooms.Categories;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorEventCategoriesWriter : NetworkPacketWriter
{
    public NavigatorEventCategoriesWriter(List<RoomCategory> categories)
    {
        WriteShort(ServerPacketId.NavigatorEventCategories);
        WriteInteger(categories.Count);

        foreach (var category in categories)
        {
            WriteLong(category.Id);
            WriteString(category.Caption);
            WriteBool(category.Visible);
        }
    }
}