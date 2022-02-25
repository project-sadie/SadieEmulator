using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorCollapsedCategoriesWriter : NetworkPacketWriter
{
    public NavigatorCollapsedCategoriesWriter(List<string> categories) : base(ServerPacketId.NavigatorCollapsedCategories)
    {
        WriteInt(categories.Count);

        foreach (var category in categories)
        {
            WriteString(category);
        }
    }
}