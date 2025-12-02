using System.Drawing;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Furniture;
using Sadie.API.Interfaces.Game.Rooms.Furniture.Processors;
using Sadie.API.Interfaces.Game.Rooms.Mapping;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.API.Interfaces.Networking;
using Sadie.Core.Enums.Game.Furniture;
using Sadie.Core.Enums.Game.Rooms.Mapping;
using Sadie.Core.Enums.Game.Rooms.Users;
using Sadie.Networking.Packets.Writers.Rooms.Furniture;

namespace Sadie.Game.Rooms.Furniture.Processors;

public class RollerProcessor(IRoomTileMapHelperService tileMapHelperService,
    IRoomFurnitureItemHelperService roomFurnitureItemHelperService) : IRoomFurnitureItemProcessor
{
    public async Task<IEnumerable<AbstractPacketWriter>> GetUpdatesForRoomAsync(IRoomLogic room)
    {
        var writers = new List<AbstractPacketWriter>();
        
        var roomRollers = room
            .Room.FurnitureItems
            .Where(x => x.PlayerFurnitureItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.Roller);
        
        var rollerUpdates = await GetRollerUpdatesAsync(room, roomRollers);
        
        writers.AddRange(rollerUpdates);
        return writers;
    }

    private async Task<IEnumerable<RoomObjectsRollingWriter>> GetRollerUpdatesAsync(
        IRoomLogic room, 
        IEnumerable<PlayerFurnitureItemPlacementDataDto> rollers)
    {
        var writers = new List<RoomObjectsRollingWriter>();
        var userIdsProcessed = new HashSet<long>();
        var itemIdsProcessed = new HashSet<int>();

        foreach (var roller in rollers)
        {
            var x = roller.PositionX;
            var y = roller.PositionY;
            
            var nextStep = tileMapHelperService.GetPointInFront(x, y, roller.Direction);
            
            var rollerPosition = new Point(x, y);
            
            var nextRoller = tileMapHelperService
                .GetItemsForPosition(nextStep.X, nextStep.Y, room.Room.FurnitureItems)
                .FirstOrDefault(fi => fi.PlayerFurnitureItem.FurnitureItem.InteractionType == FurnitureItemInteractionType.Roller);
            
            var nextHeight = nextRoller?.PlayerFurnitureItem.FurnitureItem?.StackHeight ?? 0;
            
            var users = room.UserRepository
                .GetAll()
                .Where(u => !userIdsProcessed.Contains(u.Player.Player.Id));

            var nextStepOpen = room.TileMap.TileExists(nextStep) &&
                room.TileMap.Map[nextStep.Y, nextStep.X] == (int)RoomTileState.Open &&
                !room.TileMap.UsersAtPoint(nextStep);

            if (!nextStepOpen)
            {
                continue;
            }
            
            var rollingUsers = tileMapHelperService.GetUsersAtPoints([rollerPosition], users);
            
            foreach (var rollingUser in rollingUsers)
            {
                await MoveUserOnRollerAsync(
                    x, 
                    y, 
                    nextStep, 
                    userIdsProcessed, 
                    rollingUser, 
                    writers, 
                    room, 
                    roller,
                    nextRoller, 
                    nextHeight);
            }

            var unprocessedNonRollers = room.Room.FurnitureItems.Where(i =>
                !itemIdsProcessed.Contains(i.Id) && i.PlayerFurnitureItem.FurnitureItem.InteractionType !=
                FurnitureItemInteractionType.Roller);

            var nonRollerItemsOnRoller = tileMapHelperService.GetItemsForPosition(
                roller.PositionX, 
                roller.PositionY,
                unprocessedNonRollers);
            
            if (nonRollerItemsOnRoller.Count == 0)
            {
                continue;
            }
            
            foreach (var item in nonRollerItemsOnRoller)
            {
                var furnitureItem = item.PlayerFurnitureItem.FurnitureItem;
                
                var oldPoints = tileMapHelperService.GetPointsForPlacement(
                    item.PositionX, 
                    item.PositionY, 
                    furnitureItem.TileSpanX,
                    furnitureItem.TileSpanY, 
                    item.Direction);
                
                MoveItemOnRoller(
                    nextStep,
                    itemIdsProcessed,
                    writers,
                    item,
                    roller,
                    nextHeight);

                tileMapHelperService.UpdateTileMapsForPoints(oldPoints, 
                    room.TileMap,
                    room.Room.FurnitureItems);

                var newPoints = tileMapHelperService.GetPointsForPlacement(
                    nextStep.X, nextStep.Y, 
                    furnitureItem.TileSpanX,
                    furnitureItem.TileSpanY, 
                    item.Direction);

                tileMapHelperService.UpdateTileMapsForPoints(newPoints, 
                    room.TileMap,
                    room
                        .Room.FurnitureItems
                        .Except([item])
                        .ToList());
                
                await roomFurnitureItemHelperService.BroadcastItemUpdateToRoomAsync(room, item);
            }
        }

        return writers;
    }
    
    private void MoveItemOnRoller(
        Point nextStep,
        ISet<int> itemIdsProcessed,
        ICollection<RoomObjectsRollingWriter> writers,
        PlayerFurnitureItemPlacementDataDto item,
        PlayerFurnitureItemPlacementDataDto roller,
        double nextHeight)
    {
        var rollingData = new RoomRollingObjectData
        {
            Id = item.PlayerFurnitureItemId,
            Height = item.PositionZ.ToString(),
            NextHeight = nextHeight.ToString()
        };
        
        writers.Add(new RoomObjectsRollingWriter
        {
            X = item.PositionX,
            Y = item.PositionY,
            NextX = nextStep.X,
            NextY = nextStep.Y,
            Objects = [rollingData],
            RollerId = roller.PlayerFurnitureItemId,
            MovementType = 2,
            RoomUserId = 0,
            Height = roller.PositionZ.ToString(),
            NextHeight = nextHeight.ToString()
        });
                
        item.PositionX = nextStep.X;
        item.PositionY = nextStep.Y;
        item.PositionZ = nextHeight;
                
        itemIdsProcessed.Add(item.PlayerFurnitureItemId);
    }

    private static async Task MoveUserOnRollerAsync(
        int x,
        int y,
        Point nextStep,
        ISet<long> playerIdsProcessed,
        IRoomUser rollingUser,
        ICollection<RoomObjectsRollingWriter> writers,
        IRoomLogic room,
        PlayerFurnitureItemPlacementDataDto roller,
        PlayerFurnitureItemPlacementDataDto? nextRoller,
        double nextHeight)
    {
        playerIdsProcessed.Add(rollingUser.Player.Player.Id);

        if (rollingUser.StatusMap.ContainsKey(RoomUserStatus.Move))
        {
            return;
        }

        writers.Add(new RoomObjectsRollingWriter
        {
            X = x,
            Y = y,
            NextX = nextStep.X,
            NextY = nextStep.Y,
            Objects = [],
            RollerId = roller.PlayerFurnitureItemId,
            MovementType = 2,
            RoomUserId = rollingUser.Player.Player.Id,
            Height = rollingUser.PointZ.ToString(),
            NextHeight = nextHeight.ToString()
        });

        room.TileMap.UnitMap[rollingUser.Point].Remove(rollingUser);
        room.TileMap.AddUnitToMap(nextStep, rollingUser);

        await rollingUser.SetPositionAsync(nextStep);
        
        rollingUser.PointZ = nextRoller?
            .PlayerFurnitureItem
            .FurnitureItem
            .StackHeight ?? 0;
    }
}