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
    IRoomRepository roomRepository) : INetworkPacketEventHandler
{
    public int Id => EventHandlerIds.PlayerCreateRoom;

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        eventParser.Parse(reader);

        var layoutId = await roomRepository.GetLayoutIdFromNameAsync(eventParser.LayoutName);
        
        if (layoutId == -1)
        {
            return;
        }

        var roomId = await roomRepository.CreateRoomAsync(
            eventParser.Name, 
            layoutId, 
            client.Player.Data.Id, 
            eventParser.MaxUsersAllowed, 
            eventParser.Description);

        var roomSettings = new RoomSettings
        {
            RoomId = roomId
        };

        dbContext.Set<RoomSettings>().Add(roomSettings);

        var paintSettings = new RoomPaintSettings
        {
            RoomId = roomId
        };

        dbContext.Set<RoomPaintSettings>().Add(paintSettings);
        await dbContext.SaveChangesAsync();

        var (madeRoom, room) = await roomRepository.TryLoadRoomByIdAsync(roomId);

        if (madeRoom && room != null)
        {
            await client.WriteToStreamAsync(new RoomCreatedWriter(room.Id, room.Name).GetAllBytes());
        }
    }
}