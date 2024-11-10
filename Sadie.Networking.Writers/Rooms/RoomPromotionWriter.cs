using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms;

[PacketId(ServerPacketId.RoomPromotion)]
public class RoomPromotionWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; set; }
    public required int Unknown2 { get; set; }
    public required string Unknown3 { get; set; }
    public required int Unknown4 { get; set; }
    public required int Unknown5 { get; set; }
    public required string Unknown6 { get; set; }
    public required string Unknown7 { get; set; }
    public required int Unknown8 { get; set; }
    public required int Unknown9 { get; set; }
    public required int Unknown10 { get; set; }
}