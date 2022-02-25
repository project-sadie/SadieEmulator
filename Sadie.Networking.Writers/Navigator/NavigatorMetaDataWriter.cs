using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorMetaDataWriter : NetworkPacketWriter
{
    public NavigatorMetaDataWriter(Dictionary<string, int> metaData) : base(ServerPacketId.NavigatorMetaData)
    {
        WriteInt(metaData.Count);

        foreach (var (key, value) in metaData)
        {
            WriteString(key);
            WriteInt(value);
        }
    }
}