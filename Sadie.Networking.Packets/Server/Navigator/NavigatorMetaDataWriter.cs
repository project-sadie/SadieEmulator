namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorMetaDataWriter : NetworkPacketWriter
{
    internal NavigatorMetaDataWriter() : base(ServerPacketId.NavigatorMetaData)
    {
        WriteInt(0);
    }
}