namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorSettingsWriter : NetworkPacketWriter
{
    public NavigatorSettingsWriter() : base(ServerPacketId.NavigatorSettings)
    {
        // TODO: Pass structure in 
        
        WriteInt(100);
        WriteInt(100);
        WriteInt(425);
        WriteInt(535);
        WriteBoolean(false);
        WriteInt(0);
    }
}