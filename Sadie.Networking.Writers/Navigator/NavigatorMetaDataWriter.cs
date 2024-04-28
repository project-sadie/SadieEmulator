using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorMetaData)]
public class NavigatorMetaDataWriter : AbstractPacketWriter
{
    public required Dictionary<string, int> MetaData { get; init; }
}