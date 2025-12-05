using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.DTOs.Players.Furniture;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Networking.Client;
using Sadie.API.Interfaces.Networking.Events.Handlers;
using Sadie.Core.Shared.Attributes;
using Sadie.Db;
using Sadie.Db.Models.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDelete)]
public class RoomDeleteEventHandler(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IMapper mapper,
    IPlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public required int RoomId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = await RoomHelpers.TryLoadRoomByIdAsync(
            RoomId, 
            roomRepository, 
            dbContextFactory, 
            mapper);

        if (room == null || room.Room.OwnerId != client.Player.Player.Id)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.ExecuteSqlRawAsync("UPDATE player_data SET home_room_id = NULL WHERE home_room_id = {0}", RoomId);

        var updateMap = new Dictionary<IPlayerLogic, List<PlayerFurnitureItemDto>>();

        foreach (var item in room.Room.FurnitureItems)
        {
            var playerItem = item.PlayerFurnitureItem;
            playerItem.PlacementData = null;
            dbContext.Entry(item).State = EntityState.Deleted;
            
            var onlineOwner = playerRepository.GetPlayerLogicById(item.PlayerFurnitureItem.PlayerId);

            if (onlineOwner == null)
            {
                continue;
            }
            
            if (!updateMap.ContainsKey(onlineOwner))
            {
                updateMap[onlineOwner] = [];
            }

            updateMap[onlineOwner].Add(item.PlayerFurnitureItem);
        }

        if (!roomRepository.TryRemove(RoomId, out _))
        {
            return;
        }

        var roomEntity = mapper.Map<Room>(room);
        var roomDto = mapper.Map<RoomDto>(room);
        
        dbContext.Entry(roomEntity).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();

        client.Player.Player.Rooms.Remove(roomDto);
                
        foreach (var roomUser in room.UserRepository.GetAll())
        {
            await room.UserRepository.TryRemoveAsync(roomUser.Player.Player.Id);
        }
    }
}