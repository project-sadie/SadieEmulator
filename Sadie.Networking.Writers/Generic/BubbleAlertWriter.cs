using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Generic;

[PacketId(ServerPacketId.BubbleAlert)]
public class BubbleAlertWriter : AbstractPacketWriter
{
    public required string Key { get; init; }
    public required Dictionary<string, string> Messages { get; init; }
}