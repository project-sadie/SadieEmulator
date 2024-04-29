using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.HotelView;

[PacketId(ServerPacketId.HotelViewData)]
public class HotelViewDataWriter : AbstractPacketWriter
{
    public required string Key { get; init; }
    public required string Value { get; init; }
}