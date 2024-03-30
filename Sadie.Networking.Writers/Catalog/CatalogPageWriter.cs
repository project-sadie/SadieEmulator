using Sadie.Game.Catalog.Items;
using Sadie.Game.Furniture;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

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

        #region layout_dependent
        switch (pageLayout)
        {
            case "vip_buy":
            case "club_buy":
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(0);
                break;
            case "frontpage":
                WriteString("frontpage4");
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(secondaryText);
                WriteString(teaserText);
                break;
            case "default_3x3":
                WriteString(pageLayout);
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
        #endregion
        
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
                WriteString(EnumHelpers.GetEnumDescription(furnitureItem.Type));

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
            WriteBool(item.Amount != 1);
            WriteBool(false); // unknown
            WriteString($"{item.Name}.png");
        }
        
        WriteInteger(0);
        WriteBool(false);

        if (pageLayout is "frontpage")
        {
            // TODO: serialize extra?
            WriteInteger(0);
        }
    }
}