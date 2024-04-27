using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;
using Sadie.Shared.Unsorted.Networking.Packets.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerIgnoredUsers)]
public class PlayerIgnoredUsersWriter : AbstractPacketWriter
{
    public required List<string> IgnoredUsernames { get; init; }
}