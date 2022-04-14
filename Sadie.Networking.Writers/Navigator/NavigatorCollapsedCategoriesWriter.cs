using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorCollapsedCategoriesWriter : NetworkPacketWriter
{
    public NavigatorCollapsedCategoriesWriter(List<string> categories)
    {
        WriteShort(ServerPacketId.NavigatorCollapsedCategories);
        WriteInteger(categories.Count);

        foreach (var category in categories)
        {
            WriteString(category);
        }
    }
}