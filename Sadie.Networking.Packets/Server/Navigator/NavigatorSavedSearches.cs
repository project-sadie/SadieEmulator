namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorSavedSearches : NetworkPacketWriter
{
    public NavigatorSavedSearches() : base(ServerPacketIds.NavigatorSavedSearches)
    {
        WriteInt(0);
    }
}