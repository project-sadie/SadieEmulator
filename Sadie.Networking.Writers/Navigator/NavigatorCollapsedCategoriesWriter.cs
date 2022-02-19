namespace Sadie.Networking.Packets.Server.Navigator;

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