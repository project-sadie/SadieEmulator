using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerIgnoredUsers)]
public class PlayerIgnoredUsersWriter : AbstractPacketWriter
{
    public required List<string> IgnoredUsernames { get; init; }
}