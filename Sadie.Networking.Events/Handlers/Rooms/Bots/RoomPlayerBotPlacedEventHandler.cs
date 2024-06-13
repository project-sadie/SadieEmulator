using System.Drawing;
using Sadie.Database;
using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Bots;
using Sadie.Game.Rooms.Packets.Writers;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Serialization.Attributes;
using Sadie.Networking.Writers.Players.Inventory;
using Sadie.Networking.Writers.Rooms.Bots;
using Sadie.Shared.Unsorted.Game.Rooms;

namespace Sadie.Networking.Events.Handlers.Rooms.Bots;

[PacketId(EventHandlerIds.RoomPlayerBotPlaced)]
public class RoomPlayerBotPlacedEventHandler(
    SadieContext dbContext, 
    RoomRepository roomRepository,
    RoomBotFactory roomBotFactory) : INetworkPacketEventHandler
{
    public required int Id { get; init; }
    public required int X { get; init; }
    public required int Y { get; init; }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!NetworkPacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        var bot = client.Player.Bots.FirstOrDefault(x => x.Id == Id);

        if (bot == null)
        {
            return;
        }

        if (room.OwnerId != roomUser.Id)
        {
            return;
        }

        var placePoint = new Point(X, Y);
        
        if (room.TileMap.UserMap.ContainsKey(placePoint) && room.TileMap.UserMap[placePoint].Count > 0 && !room.Settings.CanUsersOverlap)
        {
            await client.WriteToStreamAsync(new RoomBotErrorWriter
            {
                ErrorCode = 3
            });
            
            return;
        }

        var roomBot = RoomHelpersDirty.CreateBot(room.MaxUsersAllowed + bot.Id, room, new Point(X, Y), roomBotFactory);

        if (!room.BotRepository.TryAdd(roomBot))
        {
            return;
        }

        bot.RoomId = room.Id;

        dbContext.Entry(bot).Property(x => x.RoomId).IsModified = true;
        await dbContext.SaveChangesAsync();

        room.TileMap.AddBotToMap(new Point(X, Y), roomBot);
        
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