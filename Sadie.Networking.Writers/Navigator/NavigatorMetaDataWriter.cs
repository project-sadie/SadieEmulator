using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorMetaData)]
public class NavigatorMetaDataWriter : AbstractPacketWriter
{
    public required Dictionary<string, int> MetaData { get; init; }
}