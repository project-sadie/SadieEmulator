using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Generic;

[PacketId(ServerPacketId.BubbleAlert)]
public class BubbleAlertWriter : AbstractPacketWriter
{
    public required string Key { get; init; }
    public required Dictionary<string, string> Messages { get; init; }
}