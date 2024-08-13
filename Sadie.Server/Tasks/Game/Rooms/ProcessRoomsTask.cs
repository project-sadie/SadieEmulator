using System.Drawing;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Mapping;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Serialization;
using Sadie.Networking.Writers.Rooms.Furniture;

namespace SadieEmulator.Tasks.Game.Rooms;

public class ProcessRoomsTask(RoomRepository roomRepository) : IServerTask
{
    public TimeSpan PeriodicInterval => TimeSpan.FromMilliseconds(500);
    public DateTime LastExecuted { get; set; }

    public Task ExecuteAsync()
    {
        Parallel.ForEach(roomRepository.GetAllRooms(), RunPeriodicChecksForRoom);
        return Task.CompletedTask;
    }

    private static async void RunPeriodicChecksForRoom(RoomLogic? room)
    {
        if (room == null)
        {
            return;
        }

        #region Rollers
        var roomRollers = room
            .FurnitureItems
            .Where(x => x.FurnitureItem.InteractionType == "roller");

        var writers = new List<AbstractPacketWriter>();
        var userIdsProcessed = new List<int>();

        foreach (var roller in roomRollers)
        {
            var x = roller.PositionX;
            var y = roller.PositionY;
            
            var nextStep = RoomTileMapHelpers.GetPointInFront(x, y, roller.Direction);
            var rollerPosition = new Point(x, y);
            
            var nextRoller = RoomTileMapHelpers
                .GetItemsForPosition(nextStep.X, nextStep.Y, room.FurnitureItems)
                .FirstOrDefault(x => x.FurnitureItem.InteractionType == "roller");

            var users = room.UserRepository
                .GetAll()
                .Where(x => !userIdsProcessed.Contains(x.Id));
            
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
            
            var rollingData = new List<RoomRollingObjectData>();

            var nonRollerItemsOnRoller = RoomTileMapHelpers.GetItemsForPosition(
                roller.PositionX, 
                roller.PositionY,
                room.FurnitureItems.Where(x => x.FurnitureItem.InteractionType != "roller"));
            
            foreach (var item in nonRollerItemsOnRoller)
            {
                rollingData.Add(new RoomRollingObjectData
                {
                    Id = item.Id,
                    Height = item.PositionZ.ToString(),
                    NextHeight = nextRoller?.PositionZ.ToString() ?? "0"
                });
            }

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
        
        foreach (var abstractPacketWriter in writers)
        {
            await room.UserRepository.BroadcastDataAsync(abstractPacketWriter);
        }

        #endregion

        await room.BotRepository.RunPeriodicCheckAsync();
        await room.UserRepository.RunPeriodicCheckAsync();
    }
}