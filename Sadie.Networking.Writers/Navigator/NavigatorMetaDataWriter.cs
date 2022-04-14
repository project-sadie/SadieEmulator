using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Navigator;

public class NavigatorMetaDataWriter : NetworkPacketWriter
{
    public NavigatorMetaDataWriter(Dictionary<string, int> metaData)
    {
        WriteShort(ServerPacketId.NavigatorMetaData);
        WriteInteger(metaData.Count);

        foreach (var (key, value) in metaData)
        {
            WriteString(key);
            WriteInteger(value);
        }
    }
}