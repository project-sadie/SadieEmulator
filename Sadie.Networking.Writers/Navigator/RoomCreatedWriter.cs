using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Navigator;

[PacketId(ServerPacketId.RoomCreated)]
public class RoomCreatedWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
    public required string Name { get; init; }
}