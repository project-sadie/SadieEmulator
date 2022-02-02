namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorEventCategories : NetworkPacketWriter
{
    public NavigatorEventCategories() : base(ServerPacketId.NavigatorEventCategories)
    {
        WriteInt(0);
    }
}