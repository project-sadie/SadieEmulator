using Sadie.Database.Models.Furniture;
using Sadie.Networking.Serialization;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPurchaseOkWriter : AbstractPacketWriter
{
    public CatalogPurchaseOkWriter(
        int id,
        string name,
        bool rented,
        int costCredits,
        int costPoints,
        int costPointsType,
        bool canGift,
        int amount,
        int clubLevel,
        bool canPurchaseBundles,
        string metadata,
        bool isLimited,
        int limitedItemSeriesSize,
        int amountLeft,
        ICollection<FurnitureItem> furnitureItems)
    {
        WriteShort(ServerPacketId.CatalogPurchaseOk);
        WriteInteger(id);
        WriteString(name);
        WriteBool(rented);
        WriteInteger(costCredits);
        WriteInteger(costPoints);
        WriteInteger(costPointsType);
        WriteBool(canGift);
        WriteInteger(furnitureItems.Count);

        foreach (var furnitureItem in furnitureItems)
        {
            WriteString(EnumHelpers.GetEnumDescription(furnitureItem.Type));

            if (furnitureItem.Type == FurnitureItemType.Badge)
            {
                WriteString(furnitureItem.Name);
            }
            else
            {
                WriteInteger(furnitureItem.AssetId);

                if (name.Contains("wallpaper_single") || name.Contains("floor_single") || name.Contains("landscape_single"))
                {
                    WriteString(name.Split("_")[2]);
                }
                else if (name.Contains("bot") && furnitureItem.Type == FurnitureItemType.Bot)
                {
                    var look = metadata.Split(";").FirstOrDefault(x => x.StartsWith("figure:"));
                    WriteString(!string.IsNullOrEmpty(look) ? look.Replace("figure:", "") : metadata);
                }
                else if (furnitureItem.Type == FurnitureItemType.Bot || name.ToLower() == "poster" || name.StartsWith("SONG "))
                {
                    WriteString(metadata);
                }
                else
                {
                    WriteString("");
                }
                
                WriteInteger(amount);
                WriteBool(isLimited);

                if (!isLimited)
                {
                    continue;
                }
                
                WriteInteger(limitedItemSeriesSize);
                WriteInteger(amountLeft);
            }
        }

        WriteInteger(clubLevel);
        WriteBool(canPurchaseBundles);
    }
}