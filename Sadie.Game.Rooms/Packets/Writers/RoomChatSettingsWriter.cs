using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers;

namespace Sadie.Game.Rooms.Packets.Writers;

[PacketId(ServerPacketId.RoomChatSettings)]
public class RoomChatSettingsWriter : AbstractPacketWriter
{
    public required int ChatType { get; init; }
    public required int ChatWeight { get; init; }
    public required int ChatSpeed { get; init; }
    public required int ChatDistance { get; init; }
    public required int ChatProtection { get; init; }
}