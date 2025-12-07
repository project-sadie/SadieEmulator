using Sadie.API;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerPerks)]
public class PlayerPerksWriter : AbstractPacketWriter
{
    public required List<IPerkData> Perks { get; init; }
}