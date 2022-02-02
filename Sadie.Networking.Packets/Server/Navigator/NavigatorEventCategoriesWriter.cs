namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorEventCategoriesWriter : NetworkPacketWriter
{
    public NavigatorEventCategoriesWriter() : base(ServerPacketId.NavigatorEventCategories)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
    }
}