using Sadie.API.Networking;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Purse;

[PacketId(ServerPacketId.PlayerActivityPointsBalance)]
public class PlayerActivityPointsBalanceWriter : AbstractPacketWriter
{
    public required Dictionary<int, long> Currencies { get; init; }
}