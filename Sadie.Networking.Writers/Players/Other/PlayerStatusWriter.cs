using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerStatus)]
public class PlayerStatusWriter : AbstractPacketWriter
{
    public required bool IsOpen { get; init; }
    public required bool IsShuttingDown { get; init; }
    public required bool IsAuthentic { get; init; }
}