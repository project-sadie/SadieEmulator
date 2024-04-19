using Sadie.Database.Models.Catalog;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class PlayerClubOffersWriter : NetworkPacketWriter
{
    public PlayerClubOffersWriter(
        IReadOnlyCollection<CatalogClubOffer> offers, 
        int windowId, 
        bool unused, 
        bool canGift,
        int remainingDays)
    {
        WriteShort(ServerPacketId.PlayerClubOffers);
        WriteInteger(offers.Count);

        foreach (var offer in offers)
        {
            var duration = TimeSpan.FromDays(offer.DurationDays);
            var months = (int) duration.TotalDays / 31;
            var days = (int) duration.TotalDays - months * 31;

            var expires = DateTime.Now
                .AddDays(duration.TotalDays)
                .AddDays(remainingDays);
            
            WriteInteger(offer.Id);
            WriteString(offer.Name);
            WriteBool(unused);
            WriteInteger(offer.CostCredits); 
            WriteInteger(offer.CostPoints);
            WriteInteger(offer.CostPointsType);
            WriteBool(offer.IsVip);
            WriteInteger((int)duration.TotalDays / 31);
            WriteInteger(days);
            WriteBool(canGift);
            WriteInteger(offer.DurationDays);
            WriteInteger(expires.Year);
            WriteInteger(expires.Month);
            WriteInteger(expires.Day);
        }
        
        WriteInteger(windowId);
    }
}