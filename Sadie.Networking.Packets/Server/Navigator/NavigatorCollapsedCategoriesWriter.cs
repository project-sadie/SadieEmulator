namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorCollapsedCategoriesWriter : NetworkPacketWriter
{
    public NavigatorCollapsedCategoriesWriter(List<string> categories) : base(ServerPacketId.NavigatorCollapsedCategories)
    {
        // TODO: Pass structure in 
        
        WriteInt(categories.Count);

        foreach (var category in categories)
        {
            WriteString(category);
        }
    }
}