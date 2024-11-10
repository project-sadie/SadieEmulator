using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.RoomCreated)]
public class RoomCreatedWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}