using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Catalog;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.PlayerClubOffers)]
public class PlayerClubOffersWriter : AbstractPacketWriter
{
    public required IReadOnlyCollection<CatalogClubOffer> Offers { get; init; }
    public required int WindowId { get; init; }
    public required bool Unused { get; init; }
    public required bool CanGift { get; init; }
    public required int RemainingDays { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Offers.Count);

        foreach (var offer in Offers)
        {
            var duration = TimeSpan.FromDays(offer.DurationDays);
            var months = (int) duration.TotalDays / 31;
            var days = (int) duration.TotalDays - months * 31;

            var expires = DateTime.Now
                .AddDays(duration.TotalDays)
                .AddDays(RemainingDays);
            
            writer.WriteInteger(offer.Id);
            writer.WriteString(offer.Name);
            writer.WriteBool(Unused);
            writer.WriteInteger(offer.CostCredits); 
            writer.WriteInteger(offer.CostPoints);
            writer.WriteInteger(offer.CostPointsType);
            writer.WriteBool(offer.IsVip);
            writer.WriteInteger((int)duration.TotalDays / 31);
            writer.WriteInteger(days);
            writer.WriteBool(CanGift);
            writer.WriteInteger(offer.DurationDays);
            writer.WriteInteger(expires.Year);
            writer.WriteInteger(expires.Month);
            writer.WriteInteger(expires.Day);
        }
        
        writer.WriteInteger(WindowId);
    }
}