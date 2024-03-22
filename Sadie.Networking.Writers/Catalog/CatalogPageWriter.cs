using Sadie.Game.Catalog.Items;
using Sadie.Game.Furniture;
using Sadie.Shared.Networking;
using Sadie.Shared.Networking.Packets;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPageWriter : NetworkPacketWriter
{
    public CatalogPageWriter(
        int pageId, 
        string pageLayout,
        string headerImage,
        string teaserImage,
        string specialImage,
        string primaryText,
        string secondaryText,
        string teaserText,
        List<CatalogItem> items, string catalogMode)
    {
        WriteShort(ServerPacketId.CatalogPage);
        WriteInteger(pageId);
        WriteString(catalogMode);
        WriteString(pageLayout);

        switch (pageLayout)
        {
            case "frontpage":
            case "club_buy":
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(secondaryText);
                WriteString(teaserText);
                break;
            default:
                WriteInteger(3);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteString(specialImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(secondaryText);
                WriteString(teaserText);
                break;
        }
        
        WriteInteger(items.Count);

        foreach (var item in items)
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
            WriteString($"{item.FurnitureItems.First().AssetName}.png");
        }
        
        WriteInteger(0);
        WriteBool(false);

        if (pageLayout is "frontpage4")
        {
            // TODO: serialize extra?
            WriteInteger(0);
        }
        else
        {
            WriteInteger(0);
        }
    }
}