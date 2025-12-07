using Sadie.API.Interfaces.Networking;
using Sadie.Core.Shared.Attributes;

namespace Sadie.Networking.Writers.HotelView;

[PacketId(ServerPacketId.HotelViewBonusRare)]
public class HotelViewBonusRareWriter : AbstractPacketWriter
{
    public required string Name { get; init; }
    public required int Id { get; init; }
    public required int Coins { get; init; }
    public required int CoinsRequiredToBuy { get; init; }
}