using Moq;
using Sadie.API.Game.Players;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Db.Models.Furniture;
using Sadie.Db.Models.Players;
using Sadie.Db.Models.Players.Furniture;
using Sadie.Db.Models.Rooms;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Tests;

public class RoomMockHelpers
{
    protected static Room MockRoomWithName(string name)
    {
        return new Room
        {
            Name = name,
            Description = ""
        };
    }
    
    protected static Room MockRoomWithTag(string tag)
    {
        return new Room
        {
            Name = "",
            Description = "",
            Tags = new List<RoomTag>
            {
                new()
                {
                    Name = tag
                }
            }
        };
    }
    
    protected static Room MockRoomWithOwner(string username)
    {
        return new Room
        {
            Name = "",
            Description = "",
            Owner = new Player
            {
                Username = username,
                Email = "",
                Data = new PlayerData
                {
                    Player = null!
                }
            }
        };
    }

    protected static PlayerFurnitureItemPlacementData MockFurnitureItemPlacementData(string interactionType, int x = 0, int y = 0, int z = 0, bool walkable = false) =>
        new()
        {
            PlayerFurnitureItem = new PlayerFurnitureItem
            {
                Player = new Mock<Player>().Object,
                FurnitureItem = new FurnitureItem
                {
                    InteractionType = interactionType,
                    CanWalk = walkable,
                    Name = "",
                    AssetName = ""
                },
                LimitedData = "",
                MetaData = ""
            },
            PositionX = x,
            PositionY = y,
            PositionZ = z
        };

    protected static IRoomLogic MockRoomWithUserRepoAndFurniture(
        string heightMap,
        List<PlayerFurnitureItemPlacementData> furnitureItems,
        List<IRoomUser>? users = null)
    {
        var room = new Mock<IRoomLogic>();
        var roomUserRepo = new Mock<IRoomUserRepository>();
        
        room
            .SetupGet(x => x.FurnitureItems)
            .Returns(furnitureItems);

        roomUserRepo
            .Setup(x => x.GetAll())
            .Returns(users ?? []);

        var tileMap = new RoomTileMap(heightMap, room.Object.FurnitureItems);
        
        if (users != null)
        {
            foreach (var user in users)
            {
                tileMap.AddUnitToMap(user.Point, user);
            }
        }
        
        room
            .SetupGet(x => x.UserRepository)
            .Returns(roomUserRepo.Object);

        room.SetupGet(x => x.TileMap)
            .Returns(tileMap);
        
        return room.Object;
    }

    protected static IRoomUser MockRoomUser()
    {
        var player = new Mock<IPlayerLogic>();
        
        player
            .SetupGet(x => x.Id)
            .Returns(1);
        
        var roomUser = new Mock<IRoomUser>();
        
        roomUser
            .SetupGet(x => x.StatusMap)
            .Returns([]);
        
        roomUser.SetupGet(x => x.Player)
            .Returns(player.Object);
        
        return roomUser.Object;
    }
}