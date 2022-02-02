namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorSettings : NetworkPacketWriter
{
    public NavigatorSettings() : base(ServerPacketId.NavigatorSettings)
    {
        WriteInt(100);
        WriteInt(100);
        WriteInt(425);
        WriteInt(535);
        WriteBoolean(false);
        WriteInt(0);
    }
}