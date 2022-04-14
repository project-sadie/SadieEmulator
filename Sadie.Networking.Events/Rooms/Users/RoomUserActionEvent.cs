using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserActionEvent : INetworkPacketEvent
{
    private readonly IRoomRepository _roomRepository;

    public RoomUserActionEvent(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(_roomRepository, client, out var room, out var roomUser))
        {
            return;
        }
        
        var action = (RoomUserAction) reader.ReadInteger();

        if (action == RoomUserAction.Idle)
        {
            if (!roomUser.IsIdle)
            {
                roomUser.LastAction -= roomUser.IdleTime;
            }
            
            await room!.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter(roomUser).GetAllBytes());
            return;
        }

        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser.Id, action).GetAllBytes());
    }
}