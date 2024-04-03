using Sadie.Game.Catalog.FrontPage;
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
        string detailsText,
        List<CatalogItem> items, 
        string catalogMode,
        bool acceptSeasonCurrencyAsCredits,
        List<CatalogFrontPageItem>? frontPageItems)
    {
        WriteShort(ServerPacketId.CatalogPage);
        WriteInteger(pageId);
        WriteString(catalogMode);

        switch (pageLayout)
        {
            case "guilds":
                WriteString("guild_frontpage");
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case "guild_custom_furni":
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case "guild_forum":
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case "soundmachine":
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(2);
                WriteString(primaryText);
                WriteString(detailsText);
                break;
            case "marketplace_own_items":
            case "marketplace":
                WriteString(pageLayout);
                WriteInteger(0);
                WriteInteger(0);
                break;
            case "club_gifts":
                WriteString(pageLayout);
                WriteInteger(1);
                WriteString(headerImage);
                WriteInteger(1);
                WriteString(primaryText);
                break;
            case "info_loyalty":
                WriteString(pageLayout);
                WriteInteger(1);
                WriteString(headerImage);
                WriteInteger(1);
                WriteString(primaryText);
                WriteInteger(0);
                break;
            case "room_bundle":
            case "single_bundle":
                WriteString("single_bundle");
                WriteInteger(3);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteString("");
                WriteInteger(4);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                WriteString(secondaryText);
                break;
            case "badge_display":
            case "petcustomization":
                WriteString(pageLayout);
                WriteInteger(3);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteString(specialImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case "pets":
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(4);
                WriteString(primaryText);
                WriteString(secondaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case "bots":
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(secondaryText);
                break;
            case "vip_buy":
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
            case "default_3x3_color_grouping":
            case "spaces_new":
            case "trophies":
                WriteString(pageLayout);
                WriteInteger(3);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteString(specialImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case "default_3x3":
            case "recent_purchases":
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
            case "roomads":
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(2);
                WriteString(primaryText);
                WriteString(secondaryText);
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
        WriteBool(acceptSeasonCurrencyAsCredits);

        if (pageLayout is not "frontpage" || frontPageItems == null)
        {
            return;
        }
        
        WriteInteger(frontPageItems.Count);

        foreach (var item in frontPageItems)
        {
            WriteInteger(item.Id);
            WriteString(item.Title);
            WriteString(item.Image);
            WriteInteger((int) item.Type);

            switch (item.Type)
            {
                case CatalogFrontPageItemType.PageId:
                    WriteInteger(item.Page.Id);
                    break;
                case CatalogFrontPageItemType.PageName:
                    WriteString(item.Page.Name);
                    break;
                case CatalogFrontPageItemType.ProductName:
                    WriteString(item.ProductName);
                    break;
                default:
                    throw new Exception($"Unknown catalog front page item type {(int) item.Type}");
            }

            WriteInteger(-1);
        }
    }
}