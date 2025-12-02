using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Packets.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerPing)]
public class PlayerPingWriter : AbstractPacketWriter;