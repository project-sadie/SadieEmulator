using System.Drawing;
using Microsoft.EntityFrameworkCore;
using Sadie.API.Game.Rooms;
using Sadie.API.Game.Rooms.Bots;
using Sadie.Db;
using Sadie.Networking.Client;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms;
using Sadie.Networking.Writers.Rooms.Bots;
using Sadie.Shared.Attributes;

namespace Sadie.Networking.Events.Handlers.Rooms.Bots;

[PacketId(EventHandlerId.RoomPlayerBotPlaced)]
public class RoomPlayerBotPlacedEventHandler(
    IDbContextFactory<SadieDbContext> dbContextFactory,
    IRoomRepository roomRepository,
    IRoomBotFactory roomBotFactory) : INetworkPacketEventHandler
{
    public required int Id { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
    
    public async Task HandleAsync(INetworkClient client)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository,
                client,
                out var room,
                out var roomUser))
        {
            return;
        }

        var bot = client
            .Player!
            .Bots
            .FirstOrDefault(x => x.Id == Id);

        if (bot == null)
        {
            return;
        }

        if (room.OwnerId != roomUser.Player.Id)
        {
            return;
        }

        var placePoint = new Point(X, Y);
        
        if (room.TileMap.UsersAtPoint(placePoint) && !room.Settings.CanUsersOverlap)
        {
            await client.WriteToStreamAsync(new RoomBotErrorWriter
            {
                ErrorCode = 3
            });
            
            return;
        }

        var roomBot = roomBotFactory.Create(
            room, 
            room.MaxUsersAllowed + bot.Id, 
            new Point(X, Y), 
            room.TileMap.ZMap[Y, X]);

        if (!room.BotRepository.TryAdd(roomBot))
        {
            return;
        }

        bot.RoomId = room.Id;

        await using var dbContext = await dbContextFactory.CreateDbContextAsync();
        dbContext.Entry(bot).Property(x => x.RoomId).IsModified = true;
        await dbContext.SaveChangesAsync();

        room.TileMap.AddUnitToMap(new Point(X, Y), roomBot);
        
        await room.UserRepository.BroadcastDataAsync(new RoomBotDataWriter
        {
            Bots = [roomBot]
        });

        await room.UserRepository.BroadcastDataAsync(new RoomBotStatusWriter
        {
            Bots = [roomBot]
        });
        
        await client.WriteToStreamAsync(new PlayerInventoryRemoveBotWriter
        {
            Id = bot.Id
        });
    }
}