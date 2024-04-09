using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Game.Catalog.Pages;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
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
        ICollection<CatalogItem> items, 
        string catalogMode,
        bool acceptSeasonCurrencyAsCredits,
        ICollection<CatalogFrontPageItem>? frontPageItems)
    {
        WriteShort(ServerPacketId.CatalogPage);
        WriteInteger(pageId);
        WriteString(catalogMode);

        switch (pageLayout)
        {
            case CatalogPageLayout.Guilds:
                WriteString("guild_frontpage");
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case CatalogPageLayout.GuildsCustomFurniture:
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage); 
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case CatalogPageLayout.GuildForum:
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(teaserText);
                break;
            case CatalogPageLayout.SoundMachine:
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(2);
                WriteString(primaryText);
                WriteString(detailsText);
                break;
            case CatalogPageLayout.MarketplaceOwnItems:
            case CatalogPageLayout.Marketplace:
                WriteString(pageLayout);
                WriteInteger(0);
                WriteInteger(0);
                break;
            case CatalogPageLayout.ClubGifts:
                WriteString(pageLayout);
                WriteInteger(1);
                WriteString(headerImage);
                WriteInteger(1);
                WriteString(primaryText);
                break;
            case CatalogPageLayout.InfoLoyalty:
                WriteString(pageLayout);
                WriteInteger(1);
                WriteString(headerImage);
                WriteInteger(1);
                WriteString(primaryText);
                WriteInteger(0);
                break;
            case CatalogPageLayout.RoomBundle:
            case CatalogPageLayout.SingleBundle:
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
            case CatalogPageLayout.PetCustomization:
            case CatalogPageLayout.BadgeDisplay:
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
            case CatalogPageLayout.Pets:
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
            case CatalogPageLayout.Bots:
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(detailsText);
                WriteString(secondaryText);
                break;
            case CatalogPageLayout.VipBuy:
                WriteString(pageLayout);
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(0);
                break;
            case CatalogPageLayout.FrontPage:
                WriteString("frontpage4");
                WriteInteger(2);
                WriteString(headerImage);
                WriteString(teaserImage);
                WriteInteger(3);
                WriteString(primaryText);
                WriteString(secondaryText);
                WriteString(teaserText);
                break;
            case CatalogPageLayout.Default3X3ColorGrouping:
            case CatalogPageLayout.SpacesNew:
            case CatalogPageLayout.Trophies:
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
            case CatalogPageLayout.Default3X3:
            case CatalogPageLayout.RecentPurchases:
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
            case CatalogPageLayout.RoomAds:
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

                    if (item.Name.Contains("_single_"))
                    {
                        WriteString(item.Name.Split("_")[2]);
                    }
                    else if (item.Name.Contains("bot") && furnitureItem.Type == FurnitureItemType.Bot)
                    {
                        var look = item.MetaData.Split(";").FirstOrDefault(x => x.StartsWith("figure:"));
                        WriteString(!string.IsNullOrEmpty(look) ? look.Replace("figure:", "") : item.MetaData);
                    }
                    else if (furnitureItem.Type == FurnitureItemType.Bot || item.Name.ToLower() == "poster" || item.Name.StartsWith("SONG "))
                    {
                        WriteString(item.MetaData);
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
                    WriteInteger(item.CatalogPage.Id);
                    break;
                case CatalogFrontPageItemType.PageName:
                    WriteString(item.CatalogPage.Name);
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