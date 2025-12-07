using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Messenger;

[PacketId(ServerPacketId.PlayerMessageError)]
public class PlayerMessageErrorWriter : AbstractPacketWriter
{
    public required int Error { get; init; }
    public required int TargetId { get; init; }
    public string Message { get; init; } = "";
}