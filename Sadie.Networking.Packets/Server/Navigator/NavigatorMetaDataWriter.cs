namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorMetaDataWriter : NetworkPacketWriter
{
    public NavigatorMetaDataWriter() : base(ServerPacketId.NavigatorMetaDataParserComposer)
    {
        WriteInt(0);
    }
}