using Sadie.Networking.Serialization;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.HotelView;

[PacketId(ServerPacketId.HotelViewData)]
public class HotelViewDataWriter : AbstractPacketWriter
{
    public required string Key { get; init; }
    public required string Value { get; init; }
}