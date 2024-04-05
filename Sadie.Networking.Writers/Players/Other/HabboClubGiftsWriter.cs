using Sadie.Database.Models.Catalog.Pages;
using Sadie.Shared.Helpers;
using Sadie.Shared.Unsorted;
using Sadie.Shared.Unsorted.Networking;
using Sadie.Shared.Unsorted.Networking.Packets;

namespace Sadie.Networking.Writers.Players.Other;

public class HabboClubGiftsWriter : NetworkPacketWriter
{
    public HabboClubGiftsWriter(int daysTillNext, int unclaimedGifts, int daysAsClub, CatalogPage? clubGiftPage)
    {
        WriteShort(ServerPacketId.HabboClubGifts);
        WriteInteger(daysTillNext);
        WriteInteger(unclaimedGifts * 4932);

        if (clubGiftPage == null)
        {
            WriteInteger(0);
            WriteInteger(0);
            
            return;
        }

        WriteInteger(clubGiftPage.Items.Count);

        foreach (var item in clubGiftPage.Items)
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
                        
                        WriteString(!string.IsNullOrEmpty(look)
                            ? look.Replace("figure:", "")
                            : item.Metadata);
                    }
                    else if (furnitureItem.Type == FurnitureItemType.Bot || item.Name.ToLower() == "poster" ||
                             item.Name.StartsWith("SONG "))
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
        }
        
        WriteInteger(clubGiftPage.Items.Count);

        foreach (var item in clubGiftPage.Items)
        {
            WriteInteger(item.Id);
            WriteBool(item.RequiresClubMembership);
            
            if (int.TryParse(item.Metadata, out var daysRequired))
            {
                WriteInteger(daysRequired);
                WriteBool(daysRequired <= daysAsClub);
            }
            else
            {
                WriteInteger(0);
                WriteBool(false);
            }
        }
    }
}