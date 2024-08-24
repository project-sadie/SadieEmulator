using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Rooms.Bots;

[PacketId(ServerPacketId.RoomBotError)]
public class RoomBotErrorWriter : AbstractPacketWriter
{
    public required int ErrorCode { get; init; }
    
}