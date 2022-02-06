namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorEventCategoriesWriter : NetworkPacketWriter
{
    internal NavigatorEventCategoriesWriter() : base(ServerPacketId.NavigatorEventCategories)
    {
        WriteInt(0);
    }
}