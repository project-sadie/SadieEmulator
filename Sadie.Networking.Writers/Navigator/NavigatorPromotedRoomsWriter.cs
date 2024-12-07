using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.NavigatorPromotedRooms)]
public class NavigatorPromotedRoomsWriter : AbstractPacketWriter
{
    public required int Unknown1 { get; init; }
    public required string Unknown2 { get; init; }
    public required int Unknown3 { get; init; }
    public required bool Unknown4 { get; init; }
    public required int Unknown5 { get; init; }
    public required string Unknown6 { get; init; }
    public required string Unknown7 { get; init; }
    public required int Unknown8 { get; init; }
    public required string Unknown9 { get; init; }
    public required string Unknown10 { get; init; }
    public required int Unknown11 { get; init; }
    public required int Unknown12 { get; init; }
    public required int Unknown13 { get; init; }
    public required string Unknown14 { get; init; }
}