using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Polls;

[PacketId(ServerPacketId.RoomPollOffer)]
public class RoomPollOfferWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
    public required string Type { get; init; }
    public required string Headline { get; init; }
    public required string Summary { get; init; }
}