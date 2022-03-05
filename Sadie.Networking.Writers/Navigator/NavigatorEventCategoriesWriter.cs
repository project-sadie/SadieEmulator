using Sadie.Game.Rooms.Categories;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorEventCategoriesWriter : NetworkPacketWriter
{
    public NavigatorEventCategoriesWriter(List<RoomCategory> categories) : base(ServerPacketId.NavigatorEventCategories)
    {
        WriteInt(categories.Count);

        foreach (var category in categories)
        {
            WriteLong(category.Id);
            WriteString(category.Caption);
            WriteBoolean(category.Visible);
        }
    }
}