namespace Sadie.Networking.Packets.Server.Navigator;

internal class NavigatorMetaDataWriter : NetworkPacketWriter
{
    internal NavigatorMetaDataWriter(Dictionary<string, int> metaData) : base(ServerPacketId.NavigatorMetaData)
    {
        WriteInt(metaData.Count);

        foreach (var (key, value) in metaData)
        {
            WriteString(key);
            WriteInt(value);
        }
    }
}