using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Networking.Client;
using Sadie.API.Networking.Events.Handlers;
using Sadie.Db;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Db.Models.Rooms;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomDelete)]
public class RoomDeleteEventHandler(
    IRoomRepository roomRepository,
    IDbContextFactory<SadieContext> dbContextFactory,
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

        if (room == null || room.OwnerId != client.Player.Id)
        {
            return;
        }

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        await dbContext.Database.ExecuteSqlRawAsync("UPDATE player_data SET home_room_id = NULL WHERE home_room_id = {0}", RoomId);

        var updateMap = new Dictionary<IPlayerLogic, List<PlayerFurnitureItem>>();

        foreach (var item in room.FurnitureItems)
        {
            var playerItem = item.PlayerFurnitureItem!;
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

        dbContext.Entry((Room) room).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();

        client.Player.Rooms.Remove((Room) room);
                
        foreach (var roomUser in room.UserRepository.GetAll())
        {
            await room.UserRepository.TryRemoveAsync(roomUser.Player.Id);
        }
    }
}