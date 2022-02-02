namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorMetaDataParser : NetworkPacketWriter
{
    public NavigatorMetaDataParser() : base(ServerPacketId.NavigatorMetaDataParserComposer)
    {
        WriteInt(0);
    }
}