using Sadie.API.Game.Rooms;
using Sadie.Database;
using Sadie.Enums.Game.Furniture;
using Sadie.Enums.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Furniture;
using Sadie.Networking.Client;
using Sadie.Networking.Serialization.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Furniture;

[PacketId(EventHandlerId.RoomItemPlaced)]
public class RoomItemPlacedEventHandler(
    SadieContext dbContext,
    IRoomRepository roomRepository,
    RoomFurnitureItemInteractorRepository interactorRepository) : INetworkPacketEventHandler
{
    public required string PlacementData { get; set; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (client.Player == null || client.RoomUser == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        if (!client.RoomUser.HasRights())
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.MissingRights);
            return;
        }
        
        var room = roomRepository.TryGetRoomById(client.Player.State.CurrentRoomId);
        
        if (room == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }
        
        var player = client.Player;
        var placementData = PlacementData.Split(" ");
        
        if (!int.TryParse(placementData[0], out var itemId))
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        var playerItem = player.FurnitureItems.FirstOrDefault(x => x.Id == itemId);

        if (playerItem == null)
        {
            await NetworkPacketEventHelpers.SendFurniturePlacementErrorAsync(client, RoomFurniturePlacementError.CantSetItem);
            return;
        }

        if (playerItem.FurnitureItem.Type == FurnitureItemType.Floor)
        {
            await RoomHelpersDirty.OnPlaceFloorItemAsync(
                placementData, 
                room, 
                client, 
                playerItem,
                itemId,
                dbContext, 
                interactorRepository);
        }
        else if (playerItem.FurnitureItem.Type == FurnitureItemType.Wall)
        {
            await RoomHelpersDirty.OnPlaceWallItemAsync(placementData,
                room,
                player,
                playerItem,
                itemId,
                client,
                dbContext,
                interactorRepository);
        }
    }
}
    