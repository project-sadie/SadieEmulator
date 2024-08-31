using Moq;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Users;
using Sadie.Database.Models.Furniture;
using Sadie.Database.Models.Players;
using Sadie.Database.Models.Players.Furniture;
using Sadie.Database.Models.Rooms;
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
                Email = ""
            }
        };
    }

    protected static PlayerFurnitureItemPlacementData MockPlacementData(string interactionType) =>
        new()
        {
            PlayerFurnitureItem = new PlayerFurnitureItem
            {
                Player = new Mock<Player>().Object,
                FurnitureItem = new FurnitureItem
                {
                    InteractionType = interactionType
                },
                LimitedData = "",
                MetaData = ""
            }
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
        var roomUser = new Mock<IRoomUser>();
        
        roomUser
            .SetupGet(x => x.StatusMap)
            .Returns([]);
        
        return roomUser.Object;
    }
}