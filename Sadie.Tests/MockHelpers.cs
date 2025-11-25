using Moq;
using Sadie.API.DTOs.Furniture;
using Sadie.API.DTOs.Player;
using Sadie.API.DTOs.Player.Furniture;
using Sadie.API.DTOs.Rooms;
using Sadie.API.Interfaces.Game.Players;
using Sadie.API.Interfaces.Game.Rooms;
using Sadie.API.Interfaces.Game.Rooms.Users;
using Sadie.Db.Models.Players;
using Sadie.Db.Models.Rooms;
using Sadie.Game.Rooms.Mapping;

namespace Sadie.Tests;

public class MockHelpers
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
                },
                Password = ""
            }
        };
    }

    protected static PlayerFurnitureItemPlacementDataDto MockFurnitureItemPlacementData(string interactionType, int x = 0, int y = 0, int z = 0, bool walkable = false) =>
        new()
        {
            PlayerFurnitureItem = new PlayerFurnitureItemDto
            {
                FurnitureItem = new FurnitureItemDto
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
        List<PlayerFurnitureItemPlacementDataDto> furnitureItems,
        List<IRoomUser>? users = null)
    {
        var roomDto = new RoomDto
        {
            FurnitureItems = furnitureItems
        };

        var mockRoomLogic = new Mock<IRoomLogic>();
        mockRoomLogic.SetupGet(x => x.Room).Returns(roomDto);

        
        var roomUserRepo = new Mock<IRoomUserRepository>();
        roomUserRepo.Setup(x => x.GetAll()).Returns(users ?? []);
        
        mockRoomLogic.SetupGet(x => x.UserRepository).Returns(roomUserRepo.Object);

        var tileMap = new RoomTileMap(heightMap, roomDto.FurnitureItems);
        mockRoomLogic.SetupGet(x => x.TileMap).Returns(tileMap);
        
        if (users != null)
        {
            foreach (var user in users)
            {
                tileMap.AddUnitToMap(user.Point, user);
            }
        }

        return mockRoomLogic.Object;
    }
    
    protected static IRoomUser MockRoomUser()
    {
        var playerData = new PlayerDto(
            1L,
            "TestUser",
            "test@example.com",
            [],
            DateTimeOffset.UtcNow,
            new PlayerDataDto(),
            new PlayerAvatarDataDto(),
            [],
            [],
            [],
            new PlayerNavigatorSettingsDto(),
            new PlayerGameSettingsDto(),
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            [],[],[]);

        var player = new Mock<IPlayerLogic>();
        player.SetupGet(x => x.Player).Returns(playerData);

        var roomUser = new Mock<IRoomUser>();

        roomUser
            .SetupGet(x => x.StatusMap)
            .Returns(new Dictionary<string, string>());

        roomUser
            .SetupGet(x => x.Player)
            .Returns(player.Object);

        return roomUser.Object;
    }
    
    public static Mock<IPlayerRepository> CreatePlayerRepositoryMock()
    {
        var mock = new Mock<IPlayerRepository>();

        mock.Setup(r => r.GetPlayerLogicById(It.IsAny<long>()))
            .Returns((long id) => new Mock<IPlayerLogic>().Object);

        return mock;
    }
}