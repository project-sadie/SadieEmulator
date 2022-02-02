namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorSavedSearchesWriter : NetworkPacketWriter
{
    public NavigatorSavedSearchesWriter() : base(ServerPacketId.NavigatorSavedSearches)
    {
        WriteInt(0);
    }
}