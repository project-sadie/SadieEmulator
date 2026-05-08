using Sadie.API;
using Sadie.API.DTOs.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Shared.Attributes;
using Sadie.Core.Shared.Helpers;

namespace Sadie.Networking.Writers.Catalog;

[PacketId(ServerPacketId.CatalogPurchaseOk)]
public class CatalogPurchaseOkWriter : AbstractPacketWriter
{
    public required int Id { get; init; }
    public required string? Name { get; init; }
    public required bool Rented { get; init; }
    public required int CostCredits { get; init; }
    public required int CostPoints { get; init; }
    public required int CostPointsType { get; init; }
    public required bool CanGift { get; init; }
    public required ICollection<FurnitureItemDto> FurnitureItems { get; init; }
    public required int Amount { get; init; }
    public required int ClubLevel { get; init; }
    public required bool CanPurchaseBundles { get; init; }
    public required string? Metadata { get; init; }
    public required bool IsLimited { get; init; }
    public required int LimitedItemSeriesSize { get; init; }
    public required int AmountLeft { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(Id);
        writer.WriteString(Name);
        writer.WriteBool(Rented);
        writer.WriteInteger(CostCredits);
        writer.WriteInteger(CostPoints);
        writer.WriteInteger(CostPointsType);
        writer.WriteBool(CanGift);
        writer.WriteInteger(FurnitureItems.Count);

        foreach (var furnitureItem in FurnitureItems)
        {
            writer.WriteString(EnumHelpers.GetEnumDescription(furnitureItem.Type));

            if (furnitureItem.Type == FurnitureItemType.Badge)
            {
                writer.WriteString(furnitureItem.Name);
            }
            else
            {
                writer.WriteInteger(furnitureItem.AssetId);

                if (Name.Contains("wallpaper_single") || Name.Contains("floor_single") || Name.Contains("landscape_single"))
                {
                    writer.WriteString(Name.Split("_")[2]);
                }
                else if (Name.Contains("bot") && furnitureItem.Type == FurnitureItemType.Bot)
                {
                    var look = Metadata.Split(";").FirstOrDefault(x => x.StartsWith("figure:"));
                    writer.WriteString(!string.IsNullOrEmpty(look) ? look.Replace("figure:", "") : Metadata);
                }
                else if (furnitureItem.Type == FurnitureItemType.Bot || Name.ToLower() == "poster" || Name.StartsWith("SONG "))
                {
                    writer.WriteString(Metadata);
                }
                else
                {
                    writer.WriteString("");
                }
                
                writer.WriteInteger(Amount);
                writer.WriteBool(IsLimited);

                if (!IsLimited)
                {
                    continue;
                }
                
                writer.WriteInteger(LimitedItemSeriesSize);
                writer.WriteInteger(AmountLeft);
            }
        }

        writer.WriteInteger(ClubLevel);
        writer.WriteBool(CanPurchaseBundles);
    }
}