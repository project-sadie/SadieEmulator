namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorMetaDataParser : NetworkPacketWriter
{
    public NavigatorMetaDataParser() : base(ServerPacketIds.NavigatorMetaDataParserComposer)
    {
        WriteInt(0);
    }
}