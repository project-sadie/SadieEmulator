namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorEventCategories : NetworkPacketWriter
{
    public NavigatorEventCategories() : base(ServerPacketIds.NavigatorEventCategories)
    {
        WriteInt(0);
    }
}