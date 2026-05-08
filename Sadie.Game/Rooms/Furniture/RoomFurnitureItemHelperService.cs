using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Enums.Miscellaneous;
using Sadie.Db;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture;

public class RoomFurnitureItemHelperService(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IPlayerRepository playerRepository,
    IMapper mapper) : IRoomFurnitureItemHelperService
{
    public async Task CycleInteractionStateForItemAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementDataDto roomFurnitureItem)
    {
        if (string.IsNullOrEmpty(roomFurnitureItem.PlayerFurnitureItem.MetaData))
        {
            roomFurnitureItem.PlayerFurnitureItem.MetaData = 0.ToString();
        }

        var furnitureItem = roomFurnitureItem.PlayerFurnitureItem.FurnitureItem;
        
        if (furnitureItem.InteractionModes < 1 ||
            !int.TryParse(roomFurnitureItem.PlayerFurnitureItem.MetaData, out var state))
        {
            return;
        }

        if (state >= furnitureItem.InteractionModes)
        {
            state = 0;
        }

        await UpdateMetaDataForItemAsync(room, roomFurnitureItem, (state + 1).ToString());
        
        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        
        var playerFurnitureItemEntity = mapper.Map<PlayerFurnitureItem>(roomFurnitureItem.PlayerFurnitureItem);
        
        dbContext
            .Entry(playerFurnitureItemEntity)
            .Property(x => x.MetaData).IsModified = true;
        
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateMetaDataForItemAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementDataDto roomFurnitureItem, 
        string metaData)
    {
        roomFurnitureItem.PlayerFurnitureItem.MetaData = metaData;
        await BroadcastItemUpdateToRoomAsync(room, roomFurnitureItem);
    }

    public async Task BroadcastItemUpdateToRoomAsync(
        IRoomLogic room, 
        PlayerFurnitureItemPlacementDataDto roomFurnitureItem)
    {
        var furnitureItem = roomFurnitureItem.PlayerFurnitureItem.FurnitureItem;

        AbstractPacketWriter itemWriter = furnitureItem.Type == FurnitureItemType.Floor ? 
            new RoomFloorItemUpdatedWriter
            {
                Id = roomFurnitureItem.PlayerFurnitureItemId,
                AssetId = furnitureItem.AssetId,
                PositionX = roomFurnitureItem.PositionX,
                PositionY = roomFurnitureItem.PositionY,
                Direction = (int)roomFurnitureItem.Direction,
                PositionZ = roomFurnitureItem.PositionZ,
                StackHeight = 0.ToString(),
                Extra = 0,
                ObjectDataKey = (int) GetObjectDataKeyForItem(roomFurnitureItem),
                ObjectData = GetObjectDataForItem(roomFurnitureItem),
                MetaData = roomFurnitureItem.PlayerFurnitureItem.MetaData,
                Expires = -1,
                InteractionModes = 1,
                OwnerId = roomFurnitureItem.PlayerFurnitureItem.PlayerId
            }
            : new RoomWallFurnitureItemUpdatedWriter
        {
            Item = roomFurnitureItem,
            OwnerUsername = await playerRepository.GetPlayerUsernameByIdAsync(
                roomFurnitureItem.PlayerFurnitureItem.PlayerId
            ) ?? "Unknown User"
        };
        
        await room.BroadcastDataAsync(itemWriter);
    }

    public ObjectDataKey GetObjectDataKeyForItem(PlayerFurnitureItemPlacementDataDto furnitureItem)
    {
        return furnitureItem.PlayerFurnitureItem.FurnitureItem.InteractionType switch
        {
            FurnitureItemInteractionType.RoomAdsBg => ObjectDataKey.MapKey,
            _ => ObjectDataKey.LegacyKey
        };
    }

    public Dictionary<string, string> GetObjectDataForItem(PlayerFurnitureItemPlacementDataDto furnitureItem)
    {
        if (furnitureItem.PlayerFurnitureItem.FurnitureItem.InteractionType != FurnitureItemInteractionType.RoomAdsBg)
        {
            return new Dictionary<string, string>();
        }
        
        var data = new Dictionary<string, string>();
            
        foreach (var piece in furnitureItem.PlayerFurnitureItem.MetaData.Split(";"))
        {
            var parts = piece.Split("=");
            var key = parts[0];
            var value = parts.Length < 2 ? "" : parts[1];

            data[key] = value;
        }

        return data;
    }
}