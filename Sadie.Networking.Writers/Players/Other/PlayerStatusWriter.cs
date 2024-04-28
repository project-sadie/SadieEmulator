using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerStatus)]
public class PlayerStatusWriter : AbstractPacketWriter
{
    public required bool IsOpen { get; init; }
    public required bool IsShuttingDown { get; init; }
    public required bool IsAuthentic { get; init; }
}