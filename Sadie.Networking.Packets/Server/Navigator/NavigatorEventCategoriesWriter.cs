using Sadie.Game.Rooms.Categories;

namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorEventCategoriesWriter : NetworkPacketWriter
{
    internal NavigatorEventCategoriesWriter(List<RoomCategory> categories) : base(ServerPacketId.NavigatorEventCategories)
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