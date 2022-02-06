namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorSavedSearchesWriter : NetworkPacketWriter
{
    internal NavigatorSavedSearchesWriter() : base(ServerPacketId.NavigatorSavedSearches)
    {
        WriteInt(0);
    }
}