using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorMetaData)]
public class NavigatorMetaDataWriter : AbstractPacketWriter
{
    public required Dictionary<string, int> MetaData { get; init; }
}