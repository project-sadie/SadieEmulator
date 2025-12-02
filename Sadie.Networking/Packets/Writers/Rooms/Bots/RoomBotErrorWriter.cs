using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Rooms.Bots;

[PacketId(ServerPacketId.RoomBotError)]
public class RoomBotErrorWriter : AbstractPacketWriter
{
    public required int ErrorCode { get; init; }
    
}