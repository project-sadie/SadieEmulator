using Sadie.Game.Rooms;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserTagsEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserTagsEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        var roomUserId = reader.ReadInt();
        
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out _))
        {
            return;
        }

        if (room != null && room.UserRepository.TryGet(roomUserId, out var specialUser))
        {        
            await specialUser!.NetworkObject.WriteToStreamAsync(new RoomUserTagsWriter(specialUser.Id, specialUser.AvatarData.Tags).GetAllBytes());
        }
    }
}