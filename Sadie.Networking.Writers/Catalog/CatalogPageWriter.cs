using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Networking.Serialization;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;

namespace Sadie.Networking.Writers.Catalog;

public class CatalogPageWriter : AbstractPacketWriter
{
    public CatalogPageWriter(
        int pageId, 
        string pageLayout,
        List<string> images,
        List<string> texts,
        ICollection<CatalogItem> items, 
        string catalogMode,
        bool acceptSeasonCurrencyAsCredits,
        ICollection<CatalogFrontPageItem>? frontPageItems)
    {
        WriteShort(ServerPacketId.CatalogPage);
        WriteInteger(pageId);
        WriteString(catalogMode);
        WriteString(pageLayout);

        WriteInteger(images.Count);

        foreach (var image in images)
        {
            WriteString(image);
        }

        WriteInteger(texts.Count);

        foreach (var text in texts)
        {
            WriteString(text);
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
            WriteInteger((int) item.TypeId);

            switch (item.TypeId)
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
                    throw new Exception($"Unknown catalog front page item type {(int) item.TypeId}");
            }

            WriteInteger(-1);
        }
    }
}