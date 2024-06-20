using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerPing)]
public class PlayerPingWriter : AbstractPacketWriter
{
}