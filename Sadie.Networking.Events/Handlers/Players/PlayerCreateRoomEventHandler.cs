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
            .FirstOrDefault(x => x.Name == eventParser.LayoutName);

        if (layout == null)
        {
            return;
        }

        var newRoom = new Room
        {
            Name = eventParser.Name,
            LayoutId = layout.Id,
            Layout = layout,
            OwnerId = client.Player.Id,
            Owner = client.Player,
            MaxUsersAllowed = 50,
            Description = eventParser.Description,
            CreatedAt = DateTime.Now
        };

        newRoom.Settings = new RoomSettings
        {
            RoomId = newRoom.Id
        };

        newRoom.ChatSettings = new RoomChatSettings
        {
            RoomId = newRoom.Id
        };

        newRoom.PaintSettings = new RoomPaintSettings
        {
            RoomId = newRoom.Id,
        };

        dbContext.Rooms.Add(newRoom);
        roomRepository.AddRoom(newRoom);
        client.Player.Rooms.Add(newRoom);

        await client.WriteToStreamAsync(new RoomCreatedWriter(newRoom.Id, newRoom.Name));
        await dbContext.SaveChangesAsync();
    }
}