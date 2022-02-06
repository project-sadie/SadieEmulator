using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Categories;

namespace Sadie.Networking.Packets.Server.Navigator;

internal class RoomCategoriesWriter : NetworkPacketWriter
{
    internal RoomCategoriesWriter(List<RoomCategory> categories) : base(ServerPacketId.RoomCategories)
    {
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