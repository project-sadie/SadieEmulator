using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;

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