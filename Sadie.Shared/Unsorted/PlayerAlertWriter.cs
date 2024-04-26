using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Shared.Unsorted;

[PacketId(ServerPacketId.PlayerAlert)]
public class PlayerAlertWriter : AbstractPacketWriter
{
    public required string Message { get; set; }
}