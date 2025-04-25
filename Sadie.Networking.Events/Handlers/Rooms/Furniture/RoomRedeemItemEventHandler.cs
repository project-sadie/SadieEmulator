using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Purse;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RedeemItem)]
public class RoomRedeemItemEventHandler(
    IDbContextFactory<SadieContext> dbContextFactory) : INetworkPacketEventHandler
{
    public required int ItemId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var player = client.Player;
        var room = client.RoomUser!.Room;

        if (player?.NetworkObject == null || 
            client.RoomUser == null || 
            room.OwnerId != client.Player!.Id)
        {
            return;
        }
        
        var roomFurnitureItem = room
            .FurnitureItems
            .FirstOrDefault(x => x.PlayerFurnitureItemId == ItemId);

        if (roomFurnitureItem == null)
        {
            return;
        }

        var allowedPrefixes = new List<string>
        {
            "CF_",
            "CFC_",
            "DF_",
            "PF_"
        };

        var assetName = roomFurnitureItem.FurnitureItem.AssetName;
        
        if (!allowedPrefixes.Any(prefix => assetName.StartsWith(prefix)))
        {
            return;
        }
        
        await room.UserRepository.BroadcastDataAsync(new RoomFloorFurnitureItemRemovedWriter
        {
            Id = roomFurnitureItem.PlayerFurnitureItemId.ToString(),
            Expired = false,
            OwnerId = 0,
            Delay = 0
        });
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(roomFurnitureItem).State = EntityState.Deleted;
        dbContext.Entry(roomFurnitureItem.PlayerFurnitureItem).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();

        if ((assetName.StartsWith("CF_") ||
            assetName.StartsWith("CFC_")) ||
            assetName.Contains("_diamond_"))
        {
            var value = int.TryParse(assetName.Split("_")[1], out var amount) ? 
                amount : 
                0;

            if (assetName.StartsWith("PF_"))
            {
                player.Data.PixelBalance += value;
                dbContext.Entry(player.Data).Property(x => x.PixelBalance).IsModified = true;
                
                await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
                {
                    Currencies = NetworkPacketEventHelpers.GetPlayerCurrencyMapFromData(player.Data)
                });
            }
            else
            {
                player.Data.CreditBalance += value;
                dbContext.Entry(player.Data).Property(x => x.CreditBalance).IsModified = true;
                
                await client.WriteToStreamAsync(new PlayerCreditsBalanceWriter
                {
                    Credits = player.Data.CreditBalance
                });
            }
        }
        else if (assetName.StartsWith("DF"))
        {
            if (!int.TryParse(assetName.Split("_")[1], out var pointsType) || 
                !int.TryParse(assetName.Split("_")[2], out var points))
            {
                return;
            }

            if (pointsType == 5 || assetName.StartsWith("CF_diamond_"))
            {
                player.Data.SeasonalBalance += points;
                dbContext.Entry(player.Data).Property(x => x.SeasonalBalance).IsModified = true;
            }
            else if (pointsType == 103)
            {
                player.Data.GotwPoints += points;
                dbContext.Entry(player.Data).Property(x => x.SeasonalBalance).IsModified = true;
            }
                
            await client.WriteToStreamAsync(new PlayerActivityPointsBalanceWriter
            {
                Currencies = NetworkPacketEventHelpers.GetPlayerCurrencyMapFromData(player.Data)
            });
        }

        await dbContext.SaveChangesAsync();
    }
}