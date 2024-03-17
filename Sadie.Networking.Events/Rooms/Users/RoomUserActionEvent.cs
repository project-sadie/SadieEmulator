using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;

namespace Sadie.Networking.Events.Rooms.Users;

public class RoomUserActionEvent(IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
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
            
            await room!.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter(roomUser.Id, roomUser.IsIdle).GetAllBytes());
            return;
        }

        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser.Id, (int) action).GetAllBytes());
    }
}