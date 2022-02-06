namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorSettingsWriter : NetworkPacketWriter
{
    internal NavigatorSettingsWriter() : base(ServerPacketId.NavigatorSettings)
    {
        WriteInt(100);
        WriteInt(100);
        WriteInt(425);
        WriteInt(535);
        WriteBoolean(false);
        WriteInt(0);
    }
}