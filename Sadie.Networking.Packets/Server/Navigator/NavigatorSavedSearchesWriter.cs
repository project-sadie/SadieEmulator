namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorSavedSearchesWriter : NetworkPacketWriter
{
    public NavigatorSavedSearchesWriter() : base(ServerPacketId.NavigatorSavedSearches)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
    }
}