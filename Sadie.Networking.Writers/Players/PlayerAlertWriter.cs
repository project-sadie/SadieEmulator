using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerAlert)]
public class PlayerAlertWriter : AbstractPacketWriter
{
    public required string Message { get; set; }
}