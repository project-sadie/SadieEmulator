using Sadie.Networking.Serialization;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Players.Purse;

[PacketId(ServerPacketId.PlayerActivityPointsBalance)]
public class PlayerActivityPointsBalanceWriter : AbstractPacketWriter
{
    public required int PixelBalance { get; init; }
    public required int SeasonalBalance { get; init; }
    public required int GotwPoints { get; init; }

    public override void OnSerialize(NetworkPacketWriter writer)
    {
        var currencies = new Dictionary<int, long>
        {
            {0, PixelBalance},
            {1, 0}, // snowflakes
            {2, 0}, // hearts
            {3, 0}, // gift points
            {4, 0}, // shells
            {5, SeasonalBalance},
            {101, 0}, // snowflakes
            {102, 0}, // unknown
            {103, GotwPoints},
            {104, 0}, // unknown
            {105, 0} // unknown
        };
        
        writer.WriteShort(ServerPacketId.PlayerActivityPointsBalance);
        writer.WriteInteger(currencies.Count);

        foreach (var (currency, value) in currencies)
        {
            writer.WriteInteger(currency);
            writer.WriteLong(value);
        }
    }
}