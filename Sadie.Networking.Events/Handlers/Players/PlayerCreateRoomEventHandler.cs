using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerCreateRoomEventHandler(
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
        
        await roomRepository.CreateRoomSettings(roomId);

        var (madeRoom, room) = await roomRepository.TryLoadRoomByIdAsync(roomId);

        if (madeRoom && room != null)
        {
            await client.WriteToStreamAsync(new RoomCreatedWriter(room.Id, room.Name).GetAllBytes());
        }
    }
}