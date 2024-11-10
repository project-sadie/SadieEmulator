using Sadie.API.Networking;
using Sadie.Database.Models.Catalog.FrontPage;
using Sadie.Database.Models.Catalog.Items;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Unsorted;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Helpers;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogPage)]
public class CatalogPageWriter : AbstractPacketWriter
{
    public required int PageId { get; init; }
    public required string? CatalogMode { get; init; }
    public required string? PageLayout { get; init; }
    public required List<string?> Images { get; init; }
    public required List<string?> Texts { get; init; }
    public required List<CatalogItem> Items { get; init; }
    public required int Unknown { get; init; }
    public required bool AcceptSeasonCurrencyAsCredits { get; init; }
    public required IEnumerable<CatalogFrontPageItem> FrontPageItems { get; init; }

    public override void OnConfigureRules()
    {
        OverrideItems();
        OverrideFrontPageItems();
    }

    private void OverrideItems()
    {
        Override(GetType().GetProperty(nameof(Items))!, writer =>
        {
            writer.WriteInteger(Items.Count);

            foreach (var item in Items)
            {
                writer.WriteInteger(item.Id);
                writer.WriteString(item.Name);
                writer.WriteBool(false);
                writer.WriteInteger(item.CostCredits);
                writer.WriteInteger(item.CostPoints);
                writer.WriteInteger(item.CostPointsType);
                writer.WriteBool(item.FurnitureItems.Any(x => x.CanGift));
                writer.WriteInteger(item.FurnitureItems.Count);

                foreach (var furnitureItem in item.FurnitureItems)
                {
                    writer.WriteString(EnumHelpers.GetEnumDescription(furnitureItem.Type));

                    if (furnitureItem.Type == FurnitureItemType.Badge)
                    {
                        writer.WriteString(furnitureItem.Name);
                    }
                    else
                    {
                        writer.WriteInteger(furnitureItem.AssetId);

                        if (item.Name.Contains("_single_"))
                        {
                            writer.WriteString(item.Name.Split("_")[2]);
                        }
                        else if (item.Name.Contains("bot") && furnitureItem.Type == FurnitureItemType.Bot)
                        {
                            var look = item.MetaData.Split(";").FirstOrDefault(x => x.StartsWith("figure:"));
                            writer.WriteString(!string.IsNullOrEmpty(look) ? look.Replace("figure:", "") : item.MetaData);
                        }
                        else if (furnitureItem.Type == FurnitureItemType.Bot ||
                                 item.Name.ToLower() == "poster" ||
                                 item.Name.StartsWith("SONG "))
                        {
                            writer.WriteString(item.MetaData);
                        }
                        else
                        {
                            writer.WriteString("");
                        }
                        
                        writer.WriteInteger(item.Amount);
                        writer.WriteBool(false);
                    }
                }

                writer.WriteInteger(item.RequiresClubMembership ? 1 : 0);
                writer.WriteBool(item.Amount == 1);
                writer.WriteBool(false);
                writer.WriteString($"{item.Name}.png");
            }
        });
    }

    private void OverrideFrontPageItems()
    {
        Override(GetType().GetProperty(nameof(FrontPageItems))!, writer =>
        {
            if (PageLayout is not "frontpage4")
            {
                return;
            }
            
            writer.WriteInteger(FrontPageItems.Count());

            foreach (var item in FrontPageItems)
            {
                writer.WriteInteger(item.Id);
                writer.WriteString(item.Title);
                writer.WriteString(item.Image);
                writer.WriteInteger((int)item.TypeId);

                switch (item.TypeId)
                {
                    case CatalogFrontPageItemType.PageId:
                        writer.WriteInteger(item.CatalogPage.Id);
                        break;
                    case CatalogFrontPageItemType.PageName:
                        writer.WriteString(item.CatalogPage.Name);
                        break;
                    case CatalogFrontPageItemType.ProductName:
                        writer.WriteString(item.ProductName);
                        break;
                    default:
                        throw new Exception($"Unknown catalog front page item type {(int)item.TypeId}");
                }

                writer.WriteInteger(-1);
            }
        });
    }
}