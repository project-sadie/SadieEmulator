using System.Drawing;
using System.Globalization;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Furniture.Processors;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Packets.Writers.Furniture;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Serialization;

namespace Sadie.Game.Rooms.Furniture.Processors;

public class RollerProcessor : IRoomFurnitureItemProcessor
{
    public async Task<IEnumerable<AbstractPacketWriter>> GetUpdatesForRoomAsync(IRoomLogic room)
    {
        var writers = new List<AbstractPacketWriter>();
        
        var roomRollers = room
            .FurnitureItems
            .Where(x => x.FurnitureItem!.InteractionType == FurnitureItemInteractionType.Roller);
        
        var rollerUpdates = await GetRollerUpdatesAsync(room, roomRollers);
        
        writers.AddRange(rollerUpdates);
        return writers;
    }

    private static async Task<IEnumerable<RoomObjectsRollingWriter>> GetRollerUpdatesAsync(
        IRoomLogic room, 
        IEnumerable<PlayerFurnitureItemPlacementData> rollers)
    {
        var writers = new List<RoomObjectsRollingWriter>();
        var usersProcessed = new HashSet<int>();
        var itemsProcessed = new HashSet<int>();

        foreach (var roller in rollers)
        {
            var x = roller.PositionX;
            var y = roller.PositionY;
            var nextStep = RoomTileMapHelpers.GetPointInFront(x, y, roller.Direction);
            
            if (!room.TileMap.TileExists(nextStep))
            {
                continue;
            }
            
            if (RoomTileMapHelpers.GetTileState(nextStep.X, nextStep.Y, room.FurnitureItems) == RoomTileState.Blocked)
            {
                continue;
            }
            
            var rollerPosition = new Point(x, y);
            
            var nextRoller = RoomTileMapHelpers
                .GetItemsForPosition(nextStep.X, nextStep.Y, room.FurnitureItems)
                .FirstOrDefault(fi => fi.FurnitureItem!.InteractionType == FurnitureItemInteractionType.Roller);
            
            var nextHeight = nextRoller?.FurnitureItem?.StackHeight ?? 0;
            
            var users = room.UserRepository
                .GetAll()
                .Where(u => !usersProcessed.Contains(u.Id));
            
            var rollingUsers = RoomTileMapHelpers.GetUsersAtPoints([rollerPosition], users);
            
            foreach (var rollingUser in rollingUsers)
            {
                MoveUserOnRoller(
                    x, 
                    y, 
                    nextStep, 
                    usersProcessed, 
                    rollingUser, 
                    writers, 
                    room, 
                    roller,
                    nextRoller, 
                    nextHeight);
            }

            var unprocessedNonRollers = room.FurnitureItems.Where(i =>
                !itemsProcessed.Contains(i.Id) && i.FurnitureItem!.InteractionType !=
                FurnitureItemInteractionType.Roller);

            var nonRollerItemsOnRoller = RoomTileMapHelpers.GetItemsForPosition(
                roller.PositionX, 
                roller.PositionY,
                unprocessedNonRollers);
            
            if (nonRollerItemsOnRoller.Count == 0 ||
                room.TileMap.UsersAtPoint(nextStep))
            {
                continue;
            }
            
            foreach (var item in nonRollerItemsOnRoller)
            {
                MoveItemOnRoller(
                    nextStep,
                    itemsProcessed,
                    writers,
                    item,
                    roller,
                    nextHeight);
                
                await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
            }
        }

        return writers;
    }
    
    private static void MoveItemOnRoller(
        Point nextStep,
        ISet<int> itemIdsProcessed,
        ICollection<RoomObjectsRollingWriter> writers,
        PlayerFurnitureItemPlacementData item,
        PlayerFurnitureItemPlacementData roller,
        double nextHeight)
    {
        var rollingData = new RoomRollingObjectData
        {
            Id = item.PlayerFurnitureItem.Id,
            Height = item.PositionZ.ToString(CultureInfo.CurrentCulture),
            NextHeight = nextHeight.ToString(CultureInfo.CurrentCulture)
        };
        
        writers.Add(new RoomObjectsRollingWriter
        {
            X = item.PositionX,
            Y = item.PositionY,
            NextX = nextStep.X,
            NextY = nextStep.Y,
            Objects = [rollingData],
            RollerId = roller.PlayerFurnitureItem.Id,
            MovementType = 2,
            RoomUserId = 0,
            Height = roller.PositionZ.ToString(CultureInfo.CurrentCulture),
            NextHeight = nextHeight.ToString(CultureInfo.CurrentCulture)
        });
                
        item.PositionX = nextStep.X;
        item.PositionY = nextStep.Y;
        item.PositionZ = nextHeight;
                
        itemIdsProcessed.Add(item.Id);
    }

    private static void MoveUserOnRoller(
        int x,
        int y,
        Point nextStep,
        ISet<int> userIdsProcessed,
        IRoomUser rollingUser,
        ICollection<RoomObjectsRollingWriter> writers,
        IRoomLogic room,
        PlayerFurnitureItemPlacementData roller,
        PlayerFurnitureItemPlacementData? nextRoller,
        double nextHeight)
    {
        userIdsProcessed.Add(rollingUser.Id);

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
            RollerId = roller.PlayerFurnitureItem.Id,
            MovementType = 2,
            RoomUserId = rollingUser.Id,
            Height = rollingUser.PointZ.ToString(),
            NextHeight = nextHeight.ToString()
        });

        room.TileMap.UserMap[rollingUser.Point].Remove(rollingUser);
        room.TileMap.AddUserToMap(nextStep, rollingUser);

        rollingUser.Point = nextStep;
        rollingUser.PointZ = nextRoller?.FurnitureItem?.StackHeight ?? 0;
    }
}