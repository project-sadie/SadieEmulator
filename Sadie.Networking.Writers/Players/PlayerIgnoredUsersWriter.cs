using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerIgnoredUsers)]
public class PlayerIgnoredUsersWriter : AbstractPacketWriter
{
    public required List<string> IgnoredUsernames { get; init; }
}