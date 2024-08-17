using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Game.Players.Packets.Writers;

[PacketId(ServerPacketId.PlayerAlert)]
public class PlayerAlertWriter : AbstractPacketWriter
{
    public required string Message { get; set; }
}