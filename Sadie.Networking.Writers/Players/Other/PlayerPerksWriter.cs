using Sadie.API;
using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerPerks)]
public class PlayerPerksWriter : AbstractPacketWriter
{
    public required List<IPerkData> Perks { get; init; }
}