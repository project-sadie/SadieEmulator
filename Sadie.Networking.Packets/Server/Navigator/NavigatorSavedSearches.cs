namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorSavedSearches : NetworkPacketWriter
{
    public NavigatorSavedSearches() : base(ServerPacketId.NavigatorSavedSearches)
    {
        WriteInt(0);
    }
}