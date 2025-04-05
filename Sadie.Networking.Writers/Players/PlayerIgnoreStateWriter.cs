using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players;

[PacketId(ServerPacketId.PlayerIgnoreState)]
public class PlayerIgnoreStateWriter : AbstractPacketWriter
{
    public required int State { get; init; }
    public required string Username { get; init; }
}