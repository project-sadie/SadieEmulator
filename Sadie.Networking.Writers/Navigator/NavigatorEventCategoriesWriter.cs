using Sadie.Game.Rooms.Categories;

namespace Sadie.Networking.Packets.Server.Navigator;

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