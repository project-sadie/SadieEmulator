using Sadie.Game.Catalog;
using Sadie.Game.Catalog.Pages;
using Sadie.Game.Furniture;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPageWriter : NetworkPacketWriter
{
    public CatalogPageWriter(CatalogPage page, string catalogMode)
    {
        WriteShort(ServerPacketId.CatalogPage);
        WriteInteger(page.Id);
        WriteString(catalogMode);

        switch (page.Layout)
        {
            case "frontpage":
                WriteString("frontpage4");
                WriteInteger(2);
                WriteString(page.HeaderImage);
                WriteString(page.TeaserImage);
                WriteInteger(3);
                WriteString(page.PrimaryText);
                WriteString(page.SecondaryText);
                WriteString(page.TeaserText);
                break;
            default:
                WriteString("default_3x3");
                WriteInteger(3);
                WriteString(page.HeaderImage);
                WriteString(page.TeaserImage);
                WriteString(page.SpecialImage);
                WriteInteger(3);
                WriteString(page.PrimaryText);
                WriteString(page.SecondaryText);
                WriteString(page.TeaserText);
                break;
        }
        
        WriteInteger(page.Items.Count);

        foreach (var item in page.Items)
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
        
        WriteInteger(0);
        WriteBool(false);

        if (page.Layout is "frontpage" or "frontpage_featured")
        {
            // TODO: serialize extra?
        }
    }
}