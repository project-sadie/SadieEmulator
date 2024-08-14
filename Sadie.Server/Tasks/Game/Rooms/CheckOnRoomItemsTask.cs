using Sadie.API.Game.Rooms;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Furniture;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Rooms.Furniture;
using Point = System.Drawing.Point;

namespace SadieEmulator.Tasks.Game.Rooms;

public class CheckOnRoomItemsTask(RoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(1000);
    public DateTime LastExecuted { get; set; }
    
    public Task ExecuteAsync()
    {
        Parallel.ForEach(roomRepository.GetAllRooms(), BroadcastItemUpdates);
        return Task.CompletedTask;
    }

    private static async void BroadcastItemUpdates(RoomLogic room)
    {
        var writersToBroadcast = GetItemUpdates(room);
        
        foreach (var writer in writersToBroadcast)
        {
            await room.UserRepository.BroadcastDataAsync(writer);
        }
    }

    private static IEnumerable<AbstractPacketWriter> GetItemUpdates(IRoomLogic room)
    {
        var writers = new List<AbstractPacketWriter>();
        
        var roomRollers = room
            .FurnitureItems
            .Where(x => x.FurnitureItem!.InteractionType == FurnitureItemInteractionType.Roller);

        writers.AddRange(GetRollerUpdates(room, roomRollers));

        return writers;
    }

    private static IEnumerable<RoomObjectsRollingWriter> GetRollerUpdates(
        IRoomLogic room, 
        IEnumerable<PlayerFurnitureItemPlacementData> rollers)
    {
        var writers = new List<RoomObjectsRollingWriter>();
        var userIdsProcessed = new List<int>();
        var itemIdsProcessed = new List<int>();

        foreach (var roller in rollers)
        {
            var x = roller.PositionX;
            var y = roller.PositionY;
            
            var nextStep = RoomTileMapHelpers.GetPointInFront(x, y, roller.Direction);
            
            if (!room.TileMap.TileExists(nextStep))
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
                .Where(u => !userIdsProcessed.Contains(u.Id));
            
            var rollingUsers = RoomTileMapHelpers.GetUsersForPoints([rollerPosition], users);
            
            foreach (var rollingUser in rollingUsers)
            {
                userIdsProcessed.Add(rollingUser.Id);

                if (rollingUser.StatusMap.ContainsKey(RoomUserStatus.Move))
                {
                    continue;
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

            var nonRollerItemsOnRoller = RoomTileMapHelpers.GetItemsForPosition(
                roller.PositionX, 
                roller.PositionY,
                room.FurnitureItems.Where(i => !itemIdsProcessed.Contains(i.Id) && i.FurnitureItem!.InteractionType != FurnitureItemInteractionType.Roller));
            
            if (nonRollerItemsOnRoller.Count == 0)
            {
                continue;
            }

            nonRollerItemsOnRoller.ForEach(MoveItemOnRollerAsync);
            
            continue;

            async void MoveItemOnRollerAsync(PlayerFurnitureItemPlacementData item)
            {
                writers.Add(new RoomObjectsRollingWriter
                {
                    X = item.PositionX,
                    Y = item.PositionY,
                    NextX = nextStep.X,
                    NextY = nextStep.Y,
                    Objects = [new RoomRollingObjectData
                        {
                            Id = item.PlayerFurnitureItem.Id,
                            Height = item.PositionZ.ToString(), 
                            NextHeight = nextHeight.ToString()
                        }
                    ],
                    RollerId = roller.PlayerFurnitureItem.Id,
                    MovementType = 2,
                    RoomUserId = 0,
                    Height = roller.PositionZ.ToString(),
                    NextHeight = nextHeight.ToString()
                });
                
                item.PositionX = nextStep.X;
                item.PositionY = nextStep.Y;
                item.PositionZ = nextHeight;
                
                itemIdsProcessed.Add(item.Id);
                
                await RoomFurnitureItemHelpers.BroadcastItemUpdateToRoomAsync(room, item);
            }
        }

        return writers;
    }
}