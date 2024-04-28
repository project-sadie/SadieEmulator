using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerPong)]
public class PlayerPongWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
}