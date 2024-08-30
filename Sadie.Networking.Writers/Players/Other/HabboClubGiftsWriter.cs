using Sadie.API;
using Sadie.API.Networking;
using Sadie.Database.Models.Catalog.Pages;
using Sadie.Enums.Game.Furniture;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Shared.Helpers;

namespace Sadie.Networking.Writers.Players.Other;

[PacketId(ServerPacketId.HabboClubGifts)]
public class HabboClubGiftsWriter : AbstractPacketWriter
{
    public required int DaysTillNext { get; init; }
    public required int UnclaimedGifts { get; init; }
    public required int DaysAsClub { get; init; }
    public required CatalogPage? ClubGiftPage { get; init; }

    public override void OnSerialize(INetworkPacketWriter writer)
    {
        writer.WriteInteger(DaysTillNext);
        writer.WriteInteger(UnclaimedGifts * 4932);

        if (ClubGiftPage == null)
        {
            writer.WriteInteger(0);
            writer.WriteInteger(0);
            
            return;
        }

        writer.WriteInteger(ClubGiftPage.Items.Count);

        foreach (var item in ClubGiftPage.Items)
        {
            writer.WriteInteger(item.Id);
            writer.WriteString(item.Name);
            writer.WriteBool(false);
            writer.WriteInteger(item.CostCredits);
            writer.WriteInteger(item.CostPoints);
            writer.WriteInteger(item.CostPointsType);
            writer.WriteBool(item.FurnitureItems.First().CanGift);
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

                    if (item.Name.Contains("wallpaper_single") || 
                        item.Name.Contains("floor_single") || 
                        item.Name.Contains("landscape_single"))
                    {
                        writer.WriteString(item.Name.Split("_")[2]);
                    }
                    else if (item.Name.Contains("bot") && furnitureItem.Type == FurnitureItemType.Bot)
                    {
                        var look = item.MetaData.Split(";").FirstOrDefault(x => x.StartsWith("figure:"));
                        
                        writer.WriteString(!string.IsNullOrEmpty(look)
                            ? look.Replace("figure:", "")
                            : item.MetaData);
                    }
                    else if (furnitureItem.Type == FurnitureItemType.Bot || item.Name.ToLower() == "poster" ||
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
        }
        
        writer.WriteInteger(ClubGiftPage.Items.Count);

        foreach (var item in ClubGiftPage.Items)
        {
            writer.WriteInteger(item.Id);
            writer.WriteBool(item.RequiresClubMembership);
            
            if (int.TryParse(item.MetaData, out var daysRequired))
            {
                writer.WriteInteger(daysRequired);
                writer.WriteBool(daysRequired <= DaysAsClub);
            }
            else
            {
                writer.WriteInteger(0);
                writer.WriteBool(false);
            }
        }
    }
}