namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorCollapsedCategoriesWriter : NetworkPacketWriter
{
    internal NavigatorCollapsedCategoriesWriter(List<string> categories) : base(ServerPacketId.NavigatorCollapsedCategories)
    {
        WriteInt(categories.Count);

        foreach (var category in categories)
        {
            WriteString(category);
        }
    }
}