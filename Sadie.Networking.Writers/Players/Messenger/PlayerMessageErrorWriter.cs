using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerMessageError)]
public class PlayerMessageErrorWriter : AbstractPacketWriter
{
    public required int Error { get; init; }
    public required int TargetId { get; init; }
    public string Message { get; init; } = "";
}