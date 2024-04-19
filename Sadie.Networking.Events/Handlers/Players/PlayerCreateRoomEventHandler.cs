using Sadie.Database;
using Sadie.Database.Models.Rooms;
using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerCreateRoomEventHandler(
    SadieContext dbContext,
    PlayerCreateRoomEventParser eventParser,
    RoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerCreateRoom;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var layout = dbContext
            .RoomLayouts
            .Select(x => new { x.Id, x.Name })
            .FirstOrDefault(x => x.Name == eventParser.LayoutName);

        if (layout == null)
        {
            return;
        }

        var newRoom = new Room
        {
            Name = eventParser.Name,
            LayoutId = layout.Id,
            OwnerId = client.Player.Id,
            Description = eventParser.Description,
            CreatedAt = DateTime.Now
        };

        dbContext.Rooms.Add(newRoom);

        await dbContext.SaveChangesAsync();

        newRoom.Settings = new RoomSettings
        {
            RoomId = newRoom.Id
        };

        newRoom.PaintSettings = new RoomPaintSettings
        {
            RoomId = newRoom.Id,
        };

        await dbContext.SaveChangesAsync();

        var room = await roomRepository.TryLoadRoomByIdAsync(newRoom.Id);

        if (room != null)
        {
            await client.WriteToStreamAsync(new RoomCreatedWriter(room.Id, room.Name));
        }
    }
}