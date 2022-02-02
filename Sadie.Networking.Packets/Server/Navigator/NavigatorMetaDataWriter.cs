namespace Sadie.Networking.Packets.Server.Navigator;

public class NavigatorMetaDataWriter : NetworkPacketWriter
{
    public NavigatorMetaDataWriter() : base(ServerPacketId.NavigatorMetaData)
    {
        // TODO: Pass structure in 
        
        WriteInt(0);
    }
}