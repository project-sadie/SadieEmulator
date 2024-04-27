using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Packets.Writers;

public class RoomUserActionWriter : AbstractPacketWriter
{
    public required int UserId { get; set; }
    public required int Action { get; set; }
}