using Sadie.API.Game.Rooms;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Enums.Game.Furniture;
using Sadie.Game.Rooms;
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

    private static List<AbstractPacketWriter> GetItemUpdates(IRoomLogic room)
    {
        var writers = new List<AbstractPacketWriter>();
        
        var roomRollers = room
            .FurnitureItems
            .Where(x => x.FurnitureItem!.InteractionType == FurnitureItemInteractionType.Roller);

        writers.AddRange(GetRollerUpdates(room, roomRollers));

        return writers;
    }

    private static IEnumerable<AbstractPacketWriter> GetRollerUpdates(
        IRoomLogic room, 
        IEnumerable<PlayerFurnitureItemPlacementData> rollers)
    {
        var writers = new List<AbstractPacketWriter>();
        var userIdsProcessed = new List<int>();

        foreach (var roller in rollers)
        {
            var x = roller.PositionX;
            var y = roller.PositionY;
            
            var nextStep = RoomTileMapHelpers.GetPointInFront(x, y, roller.Direction);
            var rollerPosition = new Point(x, y);
            
            var nextRoller = RoomTileMapHelpers
                .GetItemsForPosition(nextStep.X, nextStep.Y, room.FurnitureItems)
                .FirstOrDefault(fi => fi.FurnitureItem!.InteractionType == FurnitureItemInteractionType.Roller);

            var users = room.UserRepository
                .GetAll()
                .Where(u => !userIdsProcessed.Contains(u.Id));
            
            var rollingUsers = RoomTileMapHelpers.GetUsersForPoints([rollerPosition], users);
            
            foreach (var rollingUser in rollingUsers)
            {
                userIdsProcessed.Add(rollingUser.Id);
                
                writers.Add(new RoomObjectsRollingWriter
                {
                    X = x,
                    Y = y,
                    NextX = nextStep.X,
                    NextY = nextStep.Y,
                    Objects = [],
                    RollerId = roller.Id,
                    MovementType = 2,
                    RoomUserId = rollingUser.Id,
                    Height = rollingUser.PointZ.ToString(),
                    NextHeight = nextRoller?.PositionZ.ToString() ?? "0"
                });

                if (rollingUser.StatusMap.ContainsKey(RoomUserStatus.Move) ||
                    rollingUser.Point != rollerPosition)
                {
                    continue;
                }
                    
                room.TileMap.UserMap[rollingUser.Point].Remove(rollingUser);
                room.TileMap.AddUserToMap(nextStep, rollingUser);
                
                rollingUser.Point = nextStep;
            }

            var nonRollerItemsOnRoller = RoomTileMapHelpers.GetItemsForPosition(
                roller.PositionX, 
                roller.PositionY,
                room.FurnitureItems.Where(i => i.FurnitureItem!.InteractionType != FurnitureItemInteractionType.Roller));

            var rollingData = nonRollerItemsOnRoller
                .Select(item => new RoomRollingObjectData
                {
                    Id = item.Id, 
                    Height = item.PositionZ.ToString(), 
                    NextHeight = nextRoller?.PositionZ.ToString() ?? "0"
                })
                .ToList();

            if (rollingData.Count != 0)
            {
                writers.Add(new RoomObjectsRollingWriter
                {
                    X = x,
                    Y = y,
                    NextX = nextStep.X,
                    NextY = nextStep.Y,
                    Objects = rollingData,
                    RollerId = roller.Id,
                    MovementType = 2,
                    RoomUserId = 0,
                    Height = roller.PositionZ.ToString(),
                    NextHeight = nextRoller?.PositionZ.ToString() ?? "0"
                });
            }
        }

        return writers;
    }
}