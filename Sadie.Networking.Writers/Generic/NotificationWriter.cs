using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Generic;

[PacketId(ServerPacketId.NotificationWriter)]
public class NotificationWriter : AbstractPacketWriter
{
    public required int Type { get; init; }
    public required Dictionary<string, string> Messages { get; init; }
}