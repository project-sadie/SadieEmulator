using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sadie.Database;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Players;
using Sadie.Game.Players.Packets;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerIds.RoomDelete)]
public class RoomDeleteEventHandler(
    RoomRepository roomRepository,
    SadieContext dbContext,
    IMapper mapper,
    PlayerRepository playerRepository) : INetworkPacketEventHandler
{
    public required int RoomId { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        var room = await Game.Rooms.RoomHelpersDirty.TryLoadRoomByIdAsync(
            RoomId, 
            roomRepository, 
            dbContext, 
            mapper);

        if (room == null || room.OwnerId != client.Player.Id)
        {
            return;
        }

        var updateMap = new Dictionary<PlayerLogic, List<PlayerFurnitureItem>>();

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

        dbContext.Entry<Room>(room).State = EntityState.Deleted;
        await dbContext.SaveChangesAsync();
                
        foreach (var roomUser in room.UserRepository.GetAll())
        {
            await room.UserRepository.TryRemoveAsync(roomUser.Id, true);
        }

        await dbContext.Database.ExecuteSqlRawAsync("UPDATE player_data SET home_room_id = NULL WHERE home_room_id = {0}}", RoomId);
    }
}