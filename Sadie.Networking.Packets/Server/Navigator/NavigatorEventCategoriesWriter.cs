namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorEventCategoriesWriter : NetworkPacketWriter
{
    public NavigatorEventCategoriesWriter() : base(ServerPacketId.NavigatorEventCategories)
    {
        WriteInt(0);
    }
}