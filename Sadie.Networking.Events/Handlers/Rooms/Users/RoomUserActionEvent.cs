using Sadie.Game.Rooms;
using Sadie.Game.Rooms.Users;
using Sadie.Networking.Client;
using Sadie.Networking.Events.Parsers.Rooms.Users;
using Sadie.Networking.Packets;
using Sadie.Networking.Writers.Rooms.Users;
using Sadie.Shared.Unsorted;

namespace Sadie.Networking.Events.Handlers.Rooms.Users;

public class RoomUserActionEvent(RoomUserActionParser parser, IRoomRepository roomRepository) : INetworkPacketEvent
{
    public async Task HandleAsync(INetworkClient client, INetworkPacketReader reader)
    {
        parser.Parse(reader);

        if (!PacketEventHelpers.TryResolveRoomObjectsForClient(roomRepository, client, out var room, out var roomUser))
        {
            return;
        }

        if (parser.Action == RoomUserAction.Idle)
        {
            if (!roomUser.IsIdle)
            {
                roomUser.LastAction -= roomUser.IdleTime;
            }
            
            await room!.UserRepository.BroadcastDataAsync(new RoomUserIdleWriter(roomUser.Id, roomUser.IsIdle).GetAllBytes());
            return;
        }
        
        roomUser.UpdateLastAction();

        await room!.UserRepository.BroadcastDataAsync(new RoomUserActionWriter(roomUser.Id, (int) parser.Action).GetAllBytes());
    }
}