using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Players;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Handlers.Players;

public class PlayerCreateRoomEvent(
    PlayerCreateRoomParser parser,
    IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (parser.LayoutId == -1)
        {
            return;
        }

        var roomId = await roomRepository.CreateRoomAsync(
            parser.Name, 
            parser.LayoutId, 
            client.Player.Data.Id, 
            parser.MaxUsersAllowed, 
            parser.Description);
        
        await roomRepository.CreateRoomSettings(roomId);

        var (madeRoom, room) = await roomRepository.TryLoadRoomByIdAsync(roomId);

        if (madeRoom && room != null)
        {
            await client.WriteToStreamAsync(new RoomCreatedWriter(room.Id, room.Name).GetAllBytes());
        }
    }
}