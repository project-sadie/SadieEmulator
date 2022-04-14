using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Navigator;

namespace Sadie.Networking.Events.Players;

public class PlayerCreateRoomEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public PlayerCreateRoomEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }
    
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var name = reader.ReadString();
        var description = reader.ReadString();
        var layoutName = reader.ReadString();
        var categoryId = reader.ReadInteger();
        var maxUsersAllowed = reader.ReadInteger();
        var tradingPermission = reader.ReadInteger();
        var layoutId = await _roomRepository.GetLayoutIdFromNameAsync(layoutName);
        
        if (layoutId == -1)
        {
            Console.WriteLine(layoutName);
            return;
        }

        var roomId = await _roomRepository.CreateRoomAsync(
            name, 
            layoutId, 
            client.Player.Data.Id, 
            maxUsersAllowed, 
            description);
        
        await _roomRepository.CreateRoomSettings(roomId);

        var (madeRoom, room) = await _roomRepository.TryLoadRoomByIdAsync(roomId);

        if (madeRoom && room != null)
        {
            await client.WriteToStreamAsync(new RoomCreatedWriter(room.Id, room.Name).GetAllBytes());
        }
    }
}