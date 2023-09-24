using Sadie.Game.Catalog.Items;
using Sadie.Game.Furniture;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPurchaseOkWriter : NetworkPacketWriter
{
    public CatalogPurchaseOkWriter(CatalogItem? item)
    {
        WriteInteger(ServerPacketId.CatalogPurchaseOk);

        if (item == null)
        {
            WriteInteger(0);
            WriteString("");
            WriteBool(false);
            WriteInteger(0);
            WriteInteger(0);
            WriteInteger(0);
            WriteBool(true);
            WriteInteger(1);
            WriteString("s");
            WriteInteger(0);
            WriteString("");
            WriteInteger(1);
            WriteInteger(0);
            WriteString("");
            WriteInteger(1);
        }
        else
        {
            WriteInteger(item.Id);
            WriteString(item.Name);
            WriteBool(false);
            WriteInteger(item.CostCredits);
            WriteInteger(item.CostPoints);
            WriteInteger(item.CostPointsType);
            WriteBool(item.FurnitureItems.First().CanGift);
            WriteInteger(item.FurnitureItems.Count);

            foreach (var furnitureItem in item.FurnitureItems)
            {
                WriteString(furnitureItem.Type.ToString());

                if (furnitureItem.Type == FurnitureItemType.Badge)
                {
                    WriteString(furnitureItem.Name);
                }
                else
                {
                    WriteInteger(furnitureItem.AssetId);

                    if (item.Name.Contains("wallpaper_single") || item.Name.Contains("floor_single") || item.Name.Contains("landscape_single"))
                    {
                        WriteString(item.Name.Split("_")[2]);
                    }
                    else if (item.Name.Contains("bot") && furnitureItem.Type == FurnitureItemType.Bot)
                    {
                        var look = item.Metadata.Split(";").FirstOrDefault(x => x.StartsWith("figure:"));
                        WriteString(!string.IsNullOrEmpty(look) ? look.Replace("figure:", "") : item.Metadata);
                    }
                    else if (furnitureItem.Type == FurnitureItemType.Bot || item.Name.ToLower() == "poster" || item.Name.StartsWith("SONG "))
                    {
                        WriteString(item.Metadata);
                    }
                    else
                    {
                        WriteString("");
                    }
                    
                    WriteInteger(item.Amount);
                    WriteBool(false); // is limited
                }
            }

            WriteInteger(item.RequiresClubMembership ? 1 : 0);
            WriteBool(false); // has offer
            WriteBool(false); // unknown
            WriteString($"{item.Name}.png");
        }
    }
}